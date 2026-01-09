using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Repositories;

namespace WorkShopManagement.Issues;

public interface IIssueRepository : IRepository<Issue, Guid>
{
}
