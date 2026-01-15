using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using WorkShopManagement.CarBays;
using WorkShopManagement.Cars.Stages;
using WorkShopManagement.Cars.StorageLocations;
using WorkShopManagement.Extensions;
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
            if (existing != null)
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
                //storageLocation: storageLocation,
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

            string? cnc,
            string? cncFirewall,
            string? cncColumn,
            DateTime? dueDate,
            DateTime? deliverDate,
            DateTime? startDate,
            string? notes,
            string? missingParts,
            //StorageLocation? storageLocation,
            string? buildMaterialNumber,
            int? angleBailment,
            AvvStatus? avvStatus,
            string? pdiStatus,
            string? imageLink
            )
        {
            var car = await _carRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new UserFriendlyException("Car not found.");

            // internal methods on entity
            car.SetOwner(ownerId);
            car.SetVin(vin);
            car.SetColor(color);
            car.SetModel(modelId);
            car.SetModelYear(modelYear);
            car.SetCnc(cnc, cncFirewall, cncColumn);
            car.SetSchedule(dueDate, deliverDate, startDate);
            car.SetNotes(notes, missingParts);
            //car.SetStorageLocation(storageLocation);
            car.SetBuildMaterialNumber(buildMaterialNumber);
            car.SetAngleBailment(angleBailment);
            car.SetAvvStatus(avvStatus);
            car.SetPdiStatus(pdiStatus);
            car.SetImageLink(imageLink);
            return car;
        }


        public async Task<Car> ChangeStorageLocation(Car car, StorageLocation storageLocation)
        {
            
            // validate storage location
            if (!Enum.IsDefined(storageLocation))
            {
                throw new UserFriendlyException($"Invalid value for {nameof(storageLocation)}");
            }

            // if car stage is not Incoming, external or Scd Warehouse -> meaning stage is prod, post prod, awaiting transport or dispatched [DISABLE STORAGE CHANGE] 
            if (!car.Stage.Equals(Stage.Incoming) && !car.Stage.Equals(Stage.ExternalWarehouse) && !car.Stage.Equals(Stage.ScdWarehouse))
            {
                throw new UserFriendlyException($"Cannot change storage location when car is in <strong>{car.Stage}</strong> stage.");
            }

            // if same as current, return
            if (car.StorageLocation == storageLocation)
            {
                return car;
            }

            // change storage location
            //- no need remove if (car.Stage.Equals(Stage.Incoming) && (!car.StorageLocation.HasValue || !Enum.IsDefined(car.StorageLocation.Value)))        // this only happes if car is in incoming stage

            // TO CHECK?? If car has same stage. simply update storage location 

            if (storageLocation.Equals(StorageLocation.K2) || storageLocation.Equals(StorageLocation.TerrenceRd))
            {
                // move to SCD Warehouse Stage
                car = await ChangeStageAsync(car, Stage.ScdWarehouse);
            }
            else
            {
                // move to External Warehouse Stage
                car = await ChangeStageAsync(car, Stage.ExternalWarehouse);
            }

                
            
            car.SetStorageLocation(storageLocation);
            //await _carRepository.UpdateAsync(car, autoSave: true);
            return car;

        }

      
        public async Task<Car> ChangeStageAsync(Car car, Stage targetStage)
        {
            // Only load logistics when the target stage requires it 
            LogisticsDetail? logisticsDetail = null;
            //logisticsDetail = await _logisticsDetailRepository.FindByCarIdAsync(car.Id);
            if (targetStage == Stage.ExternalWarehouse || targetStage == Stage.ScdWarehouse || targetStage == Stage.Dispatched)
            {
                // Prefer lookup by CarId (since LogisticsDetail FK is CarId)
                logisticsDetail = await _logisticsDetailRepository.FindByCarIdAsync(
                    carId: car.Id,
                    includeDetails: false,
                    asNoTracking: true
                );
            }

            ValidateStageChange(car, targetStage, logisticsDetail);

            var oldStage = car.Stage;

            // REVIEW THIS: 
            if (oldStage == Stage.Production &&  targetStage == Stage.PostProduction)
            {
                var activeBay = await _carBayRepository.FindActiveByCarIdAsync(car.Id);

                if (activeBay != null)
                    activeBay.SetIsActive(false);
                //await _carBayRepository.UpdateAsync(activeBay!, autoSave: true);
            }

            car.SetStage(targetStage, logisticsDetail);
            //await _carRepository.UpdateAsync(car, autoSave: true);

            return car;
        }

        /// <summary>
        /// Stage change with validation that may require LogisticsDetail.
        /// </summary>
       

        private static void ValidateStageChange(Car car, Stage targetStage, LogisticsDetail? logisticsDetail)
        {
            if (targetStage == Stage.ExternalWarehouse || targetStage == Stage.ScdWarehouse)
            {

                var missingFields = new List<string>();

                if (logisticsDetail is null)
                {
                    missingFields.Add("LogisticsDetail");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(logisticsDetail.RsvaNumber))
                    {
                        missingFields.Add("LogisticsDetail.RsvaNumber");
                    }

                    if (logisticsDetail.CreStatus.Equals(CreStatus.Pending))
                    {
                        missingFields.Add("LogisticsDetail.CreStatus.Pending");
                    }
                }


                if (missingFields.Count > 0)
                {
                    var missing = missingFields.JoinAsString(", ");

                    throw new UserFriendlyException($"Cannot move car to \"{targetStage}\". Car has missing fields: {missing} ");
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

                // ANY OTHER CONDITION MANDATORY FOR DISPATCH COMES HERE
                //else
                //{
                //    if (string.IsNullOrWhiteSpace(logisticsDetail.DeliverTo))
                //        missingLabels.Add("Deliver to");

                //     //Make It required or not
                //    if (string.IsNullOrWhiteSpace(logisticsDetail.TransportDestination))
                //        missingLabels.Add("Transport destination");
                //}

                //if (!car.DeliverDate.HasValue)
                //    missingLabels.Add("Estimated release date");

                if (missingLabels.Count > 0)
                {
                    var missing = missingLabels.JoinAsString(", ");
                    throw new UserFriendlyException(
                        message: $"Cannot dispatch this car. Please provide: {missing}."
                    );
                }
            }
        }

        // Overloaded Methods 

        public async Task<Car> ChangeStorageLocation(Guid id, StorageLocation storageLocation)
        {
            var car = await _carRepository.GetAsync(id);
            car = await ChangeStorageLocation(car, storageLocation);
            return car;
        }

        public async Task<Car> ChangeStageAsync(Guid id, Stage targetStage)
        {
            // Load car. If you need Model/Owner etc, load with details.
            var car = await _carRepository.GetAsync(id);
            car = await ChangeStageAsync(car, targetStage);
            return car;
        }


    }
}
