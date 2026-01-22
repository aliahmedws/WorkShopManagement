using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using WorkShopManagement.Cars;
using WorkShopManagement.Extensions;

namespace WorkShopManagement.LogisticsDetails
{
    public class LogisticsDetailManager : DomainService
    {
        private readonly ILogisticsDetailRepository _logisticsDetailRepository;
        private readonly ICarRepository _carRepository;

        public LogisticsDetailManager(
            ILogisticsDetailRepository logisticsDetailRepository,
            ICarRepository carRepository)
        {
            _logisticsDetailRepository = logisticsDetailRepository;
            _carRepository = carRepository;
        }

        /// <summary>
        /// Creates LogisticsDetail for a Car.
        /// Enforces 1:1 relationship and Car existence.
        /// </summary>
        public async Task<LogisticsDetail> CreateAsync(
            Guid carId,
            Port port,
            string? bookingNumber)
        {
            // 1. Ensure Car exists
            var carExists = await _carRepository.AnyAsync(x => x.Id == carId);
            if (!carExists)
            {
                throw new UserFriendlyException("Car does not exist.");
            }

            // 2. Ensure LogisticsDetail does not already exist for this Car
            var existing = await _logisticsDetailRepository.FindByCarIdAsync(
                carId: carId,
                includeDetails: false,
                asNoTracking: true
            );

            if (existing != null)
            {
                throw new UserFriendlyException(
                    "Logistics details already exist for this car."
                );
            }

            // 3. Validate enum explicitly
            port.EnsureDefined(nameof(port));

            // 4. Create LogisticsDetail
            var logisticsDetail = new LogisticsDetail(
                id: GuidGenerator.Create(),
                carId: carId,
                port: port,
                bookingNumber: bookingNumber
            );

            return logisticsDetail;
        }

        /// <summary>
        /// Submit CRE (Pending → Submitted).
        /// </summary>
        /// 

        public async Task<LogisticsDetail> UpdateAsync(
            Guid id, 
            Port port,
            string? bookingNumber,

            CreStatus creStatus,
            DateTime? creSubmissionDate,
            string? rvsaNumber,

            string? clearingAgent,
            string? clearanceRemarks,
            DateTime? clearanceDate,

            DateTime? actualPortArrivalDate,
            DateTime? actualScdArrivalDate

            )
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            var car = await _carRepository.GetAsync(logisticsDetail.CarId);
            if(car == null)
            {
                throw new UserFriendlyException("Associated car not found.");
            }

            logisticsDetail.SetBookingNumber(bookingNumber);
            logisticsDetail.SetPort(port);
            logisticsDetail.SetCreStatus(creStatus);
            logisticsDetail.SetCreDetails(rvsaNumber, creSubmissionDate);
            logisticsDetail.SetClearanceDetails(clearingAgent, clearanceRemarks, clearanceDate);
            logisticsDetail.SetActualArrivals(actualPortArrivalDate, actualScdArrivalDate);

            return logisticsDetail;
        }


        public async Task<LogisticsDetail> ChangeCreStatusAsync(Guid id, CreStatus creStatus)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            creStatus.EnsureDefined(nameof(creStatus));

            // Review: CRE State Change Behavior Later
            // Review: CRE State Change Behavior LaterCreDa

            //if (creStatus.Equals(CreStatus.Pending))
            //{
            //    throw new UserFriendlyException("CRE status can only be changed to Submitted.");
            //}

            logisticsDetail.SetCreStatus(creStatus);
            return logisticsDetail;
        }

        public async Task<LogisticsDetail> SubmitCreStatus(Guid id)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            logisticsDetail.SetCreStatus(CreStatus.Submitted);
            return logisticsDetail;
        }

        public async Task<LogisticsDetail> AddOrUpdateCreDetailsAsync(
            Guid id,
            CreStatus creStatus,
            DateTime? creSubmissionDate,
            string? rvsaNumber)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);

            creStatus.EnsureDefined(nameof(creStatus));

            logisticsDetail.SetCreStatus(creStatus);
            logisticsDetail.SetCreSubmissionDate(creSubmissionDate);
            logisticsDetail.SetRvsaNumber(rvsaNumber);
            return await _logisticsDetailRepository.UpdateAsync(logisticsDetail, autoSave: true);
        }

        public async Task<LogisticsDetail> AddOrUpdateClearanceDetailsAsync(
            Guid id,
            string clearingAgent,
            string clearanceRemarks,
            DateTime clearanceDate)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            logisticsDetail.SetClearanceDetails(
                clearingAgent: clearingAgent,
                clearanceRemarks: clearanceRemarks,
                clearanceDate: clearanceDate
            );
            return logisticsDetail;
        }

        public async Task<LogisticsDetail> AddOrUpdateActualArrivalAsync(
            Guid id,
            DateTime? actualPortArrivalDate,
            DateTime? actualScdArrivalDate)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            logisticsDetail.SetActualArrivals(
                actualPortArrivalDate: actualPortArrivalDate,
                actualScdArrivalDate: actualScdArrivalDate
            );
            return await _logisticsDetailRepository.UpdateAsync(logisticsDetail, autoSave: true);
        }

        public async Task<LogisticsDetail> AddOrUpdateDeliverDetailsAsync(
            Guid id,
            DateTime? confirmedDeliverDate,
            string? deliverNotes,
            string? deliverTo,
            string? transportDestination)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            logisticsDetail.SetDeliverDetails(
                confirmedDeliverDate: confirmedDeliverDate,
                deliverNotes: deliverNotes,
                deliverTo: deliverTo,
                transportDestination: transportDestination
            );

            return logisticsDetail;
        }


        // VALIDATION METHODS 

        public async Task<bool> ValidateCreAsync(Guid id)
        {
            var logisticsDetail = await _logisticsDetailRepository.GetAsync(id);
            
            if(logisticsDetail.CreStatus != CreStatus.Submitted)
            {
                return false;
            }

            if(string.IsNullOrWhiteSpace(logisticsDetail.RvsaNumber))
            {
                return false;
            }

            if(!logisticsDetail.CreSubmissionDate.HasValue)
            {
                return false;
            }

            return true;
        }

    }
}
