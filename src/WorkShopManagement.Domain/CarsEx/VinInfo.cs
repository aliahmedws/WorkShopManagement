using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace WorkShopManagement.CarsEx
{
    public class VinInfo : Entity<Guid>
    {
        public string VinNo { get; private set; }
        public string? VinResponse { get; private set; }
        public string? RecallResponse { get; private set; }
        public DateTime? VinLastUpdated { get; private set; }
        public DateTime? RecallLastUpdated { get; private set; }



        public VinInfo(
            Guid id,
            string vinNo,
            string vinResponse,
            string recallResponse,
            DateTime? vinLastUpdated,
            DateTime? recallLastUpdated


        ) : base(id)
        {
            VinNo = vinNo ?? throw new ArgumentNullException(nameof(vinNo));
            VinResponse = vinResponse;
            RecallResponse = recallResponse;
            VinLastUpdated = vinLastUpdated;
            RecallLastUpdated = recallLastUpdated;
        }
        public void SetVin(string vinResponse, DateTime vinLastUpdated)
        {
            VinResponse = vinResponse;
            VinLastUpdated = vinLastUpdated;
        }

        public void SetRecall(string recallResponse, DateTime recallLastUpdated)
        {
            RecallResponse = recallResponse;
            RecallLastUpdated = recallLastUpdated;
        }
    }
}
