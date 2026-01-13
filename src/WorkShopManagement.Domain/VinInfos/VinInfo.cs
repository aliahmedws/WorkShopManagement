using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Timing;
using WorkShopManagement.Utils.Helpers;

namespace WorkShopManagement.VinInfos
{
    public class VinInfo : Entity<Guid>
    {
        public string VinNo { get; private set; }
        public string? VinResponse { get; private set; }
        public string? RecallResponse { get; private set; }
        public DateTime? VinLastUpdated { get; private set; }
        public DateTime? RecallLastUpdated { get; private set; }
        public string? SpecsResponse { get; private set; }
        public DateTime? SpecsLastUpdated { get; private set; }

        public string? ImagesResponse { get; private set; }
        public DateTime? ImagesLastUpdated { get; private set; }

        public VinInfo(
            Guid id,
            string vinNo,
            string? vinResponse = null,
            DateTime? vinLastUpdated = null,
            string? recallResponse = null,
            DateTime? recallLastUpdated = null,
            string? specsResponse = null,
            DateTime? specsLastUpdated = null,
            string? imagesResponse = null,
            DateTime? imagesLastUpdated = null


        ) : base(id)
        {
            VinNo = CarHelper.NormalizeAndValidateVin(vinNo);

            VinResponse = vinResponse;
            VinLastUpdated = vinLastUpdated;

            RecallResponse = recallResponse;
            RecallLastUpdated = recallLastUpdated;

            SpecsResponse = specsResponse;
            SpecsLastUpdated = specsLastUpdated;

            ImagesResponse = imagesResponse;
            ImagesLastUpdated = imagesLastUpdated;
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

        public void SetImages(string imagesResponse, DateTime imagesLastUpdated)
        {
            ImagesResponse = imagesResponse;
            ImagesLastUpdated = imagesLastUpdated;
        }

        public void SetSpecs(string specsResponse, DateTime specsLastUpdated)
        {
            SpecsResponse = specsResponse;
            SpecsLastUpdated = specsLastUpdated;
        }
    }
}
