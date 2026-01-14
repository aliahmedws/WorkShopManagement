using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using WorkShopManagement.CarBays;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.LogisticsDetails;

namespace WorkShopManagement.Cars
{
    public class CarManager : DomainService
    {
        private readonly ICarRepository _carRepository;
        private readonly ILogisticsDetailRepository _logisticsDetailRepository;
        private readonly ICarBayRepository _carBayRepository;

        public CarManager(
            ICarRepository carRepository,
            ILogisticsDetailRepository logisticsDetailRepository,
            ICarBayRepository carBayRepository)
        {
            _carRepository = carRepository;
            _logisticsDetailRepository = logisticsDetailRepository;
            _carBayRepository = carBayRepository;
        }

        /// <summary>
        /// Creates a new Car. Stage is always Incoming by design.
        /// </summary>
        public async Task<Car> CreateAsync(
            Guid id,
            Guid ownerId,
            string vin,
            string color,
            Guid modelId,
            int modelYear,
            string? cnc = null,
            string? cncFirewall = null,
            string? cncColumn = null,
            DateTime? dueDate = null,
            DateTime? deliverDate = null,
            DateTime? startDate = null,
            string? notes = null,
            string? missingParts = null,
            StorageLocation? storageLocation = null,
            string? buildMaterialNumber = null,
            int? angleBailment = null,
            AvvStatus? avvStatus = null,
            string? pdiStatus = null, 

            string? imageLink = null)
        {
            var existing = await _carRepository.FirstOrDefaultAsync(x => x.Vin == vin);
            if(existing != null)
            {
                throw new UserFriendlyException($"A car with VIN '{vin}' already exists.");
            }

            var car = new Car(
                id: id,
                ownerId: ownerId,
                vin: vin,
                color: color,
                modelId: modelId,
                modelYear: modelYear,
                cnc: cnc,
                cncFirewall: cncFirewall,
                cncColumn: cncColumn,
                dueDate: dueDate,
                deliverDate: deliverDate,
                startDate: startDate,
                notes: notes,
                missingParts: missingParts,
                storageLocation: storageLocation,
                buildMaterialNumber: buildMaterialNumber,
                angleBailment: angleBailment,
                avvStatus: avvStatus,
                pdiStatus: pdiStatus,
                imageLink: imageLink
            );

            return car;
        }

        /// <summary>
        /// Updates "normal" editable fields (not stage). Keep stage changes separate to enforce rules.
        /// </summary>
        public async Task<Car> UpdateAsync(
            Guid id,
            Guid ownerId,
            string vin,
            string color,
            Guid modelId,
            int modelYear,

            Stage stage,

            string? cnc,
            string? cncFirewall,
            string? cncColumn,
            DateTime? dueDate,
            DateTime? deliverDate,
            DateTime? startDate,
            string? notes,
            string? missingParts,
            StorageLocation? storageLocation,
            string? buildMaterialNumber,
            int? angleBailment,
            AvvStatus? avvStatus,
            string? pdiStatus, 
            string? imageLink
            )
        {
            var car = await _carRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (car == null)
            {
                throw new UserFriendlyException("Car not found.");
            }

            // internal methods on entity
            car.SetOwner(ownerId);
            car.SetVin(vin);
            car.SetColor(color);
            car.SetModel(modelId);
            car.SetModelYear(modelYear);
            await ChangeStageAsync(car.Id, stage, storageLocation);
            //car.SetMakeTrim(make, trim);
            car.SetCnc(cnc, cncFirewall, cncColumn);
            car.SetSchedule(dueDate, deliverDate, startDate);
            car.SetNotes(notes, missingParts);
            car.SetStorageLocation(storageLocation);
            car.SetBuildMaterialNumber(buildMaterialNumber);
            car.SetAngleBailment(angleBailment);
            car.SetAvvStatus(avvStatus);
            car.SetPdiStatus(pdiStatus);
            car.SetImageLink(imageLink);
            return car;
        }

        /// <summary>
        /// Stage change with validation that may require LogisticsDetail.
        /// </summary>
        public async Task<Car> ChangeStageAsync(Guid carId, Stage targetStage, StorageLocation? storageLocation = null)
        {
            // Load car. If you need Model/Owner etc, load with details.
            var car = await _carRepository.GetAsync(carId);

            // Only load logistics when the target stage requires it
            LogisticsDetail? logisticsDetail = null;
            //logisticsDetail = await _logisticsDetailRepository.FindByCarIdAsync(car.Id);
            if (targetStage == Stage.ExternalWarehouse || targetStage == Stage.Dispatched)
            {
                // Prefer lookup by CarId (since LogisticsDetail FK is CarId)
                logisticsDetail = await _logisticsDetailRepository.FindByCarIdAsync(
                    carId: carId,
                    includeDetails: false,
                    asNoTracking: true
                );
            }

            ValidateStageChange(car, targetStage, logisticsDetail, storageLocation);

            var oldStage = car.Stage;

            if (oldStage == Stage.Production &&  targetStage == Stage.PostProduction)
            {
                var activeBay = await _carBayRepository.FindActiveByCarIdAsync(carId);

                if (activeBay != null)
                    activeBay.SetIsActive(false);
                await _carBayRepository.UpdateAsync(activeBay!, autoSave: true);
            }

            car.SetStage(targetStage, logisticsDetail);
            await _carRepository.UpdateAsync(car, autoSave: true);
            return car;
        }

        private static void ValidateStageChange(Car car, Stage targetStage, LogisticsDetail? logisticsDetail, StorageLocation? storageLocation)
        {
            if (targetStage == Stage.ExternalWarehouse)
            {
                var missingFields = new List<string>();

                if (logisticsDetail is null)
                {
                    missingFields.Add("LogisticsDetail");
                }
                else if (string.IsNullOrWhiteSpace(logisticsDetail.RsvaNumber))
                {
                    missingFields.Add("LogisticsDetail.RsvaNumber");
                }

                if (!car.StorageLocation.HasValue && !storageLocation.HasValue)
                {
                    missingFields.Add("Car.StorageLocation");
                }

                if (missingFields.Count > 0)
                {
                    var missing = missingFields.JoinAsString(", ");
                    throw new UserFriendlyException($"Cannot move car to \"External Warehouse\". Car has missing fields: {missing} ");
                }
            }
             
            if (targetStage == Stage.AwaitingTransport || targetStage == Stage.Dispatched)
            {
                if (!car.AvvStatus.HasValue)
                {
                    throw new UserFriendlyException($"Car is missing \"AVV Status\". Cannot move it to Awaiting Transport.");
                }
            }

            if (targetStage == Stage.Dispatched)
            {
                var missingLabels = new List<string>();

                if (!car.AvvStatus.HasValue)
                    missingLabels.Add("AVV status");

                if (logisticsDetail is null)
                {
                    missingLabels.Add("Logistics details");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(logisticsDetail.DeliverTo))
                        missingLabels.Add("Deliver to");

                    if (string.IsNullOrWhiteSpace(logisticsDetail.TransportDestination))
                        missingLabels.Add("Transport destination");
                }

                if (!car.DeliverDate.HasValue)
                    missingLabels.Add("Estimated release date");

                if (missingLabels.Count > 0)
                {
                    var missing = missingLabels.JoinAsString(", ");
                    throw new UserFriendlyException(
                        message: $"Cannot dispatch this car. Please provide: {missing}."
                    );
                }
            }
        }
    }
}
