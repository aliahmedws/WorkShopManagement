using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using WorkShopManagement.CarModels;
using WorkShopManagement.ListItems;

namespace WorkShopManagement.CheckLists;

public class CheckListDataSeedContributor : ITransientDependency
{
    private readonly IRepository<CheckList, Guid> _checkListRepository;
    private readonly IRepository<CarModel, Guid> _carModelRepository;
    private readonly IRepository<ListItem, Guid> _listItemRepository;

    public CheckListDataSeedContributor(
        IRepository<CheckList, Guid> checkListRepository,
        IRepository<CarModel, Guid> carModelRepository,
        IRepository<ListItem, Guid> listItemRepository)
    {
        _checkListRepository = checkListRepository;
        _carModelRepository = carModelRepository;
        _listItemRepository = listItemRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var templates = GetModelTemplates();

        var carModels = await _carModelRepository.GetListAsync();
        var modelByName = carModels
            .Where(m => !string.IsNullOrWhiteSpace(m.Name))
            .ToDictionary(m => Normalize(m.Name), m => m);

        foreach (var template in templates)
        {
            if (!modelByName.TryGetValue(Normalize(template.ModelName), out var model))
            {
                throw new Exception($"CarModel not found for checklist seeding: '{template.ModelName}'.");
            }

            var checklist = await _checkListRepository.FirstOrDefaultAsync(x =>
                x.CarModelId == model.Id &&
                x.Name == template.CheckListName);

            if (checklist == null)
            {
                checklist = new CheckList(
                    id: Guid.NewGuid(),
                    name: template.CheckListName,
                    position: template.CheckListPosition,
                    carModelId: model.Id
                );

                await _checkListRepository.InsertAsync(checklist, autoSave: true);
            }

            await SeedListItemsIfMissingAsync(checklist.Id, template.Items);
        }
    }

    private static async Task EnsureStepsNotEmpty(string modelName, List<ListItemSeed> items)
    {
        if (items == null || items.Count == 0)
        {
            throw new Exception($"ListItems are empty for model '{modelName}'.");
        }
        await Task.CompletedTask;
    }

    private async Task SeedListItemsIfMissingAsync(Guid checkListId, List<ListItemSeed> items)
    {
        await EnsureStepsNotEmpty("N/A", items);

        var anyExisting = await _listItemRepository.AnyAsync(x => x.CheckListId == checkListId);
        if (anyExisting)
        {
            return;
        }

        foreach (var it in items.OrderBy(x => x.Position))
        {
            var placeholder = string.IsNullOrWhiteSpace(it.CommentPlaceholder)
                ? it.Name
                : it.CommentPlaceholder!.Trim();

            var isAttachmentRequired = it.IsSeparator ? false : it.IsAttachmentRequired;

            var entity = new ListItem(
                id: Guid.NewGuid(),
                checkListId: checkListId,
                position: it.Position,
                name: it.Name.Trim(),
                commentPlaceholder: placeholder,
                commentType: it.CommentType,
                isAttachmentRequired: isAttachmentRequired,
                isSeparator: it.IsSeparator
            );

            await _listItemRepository.InsertAsync(entity, autoSave: true);
        }
    }

    private static string Normalize(string value)
        => value.Trim().ToUpperInvariant();


    private static List<ModelTemplate> GetModelTemplates()
        => new()
        {
            new ModelTemplate(
                ModelName: "Ford F-150 Lightning",
                CheckListName: "Default",
                CheckListPosition: 1,
                Items: GetFordLightning_Items()
            ),

            new ModelTemplate(
                ModelName: "F-150 Lightning Pro EXT",
                CheckListName: "Default",
                CheckListPosition: 1,
                Items: GetFordLightning_Items()
            ),
        };

    private static List<ListItemSeed> GetFordLightning_Items()
        => new()
        {
            new(1,  "Station 0 - Receiving Compliance Audit", null, CommentType.String, false, false),
            new(2,  "Station 1A", null, CommentType.String, false, false),
            new(3,  "Station 1B", null, CommentType.String, false, false),
            new(4,  "Station 2", null, CommentType.String, false, false),
            new(5,  "Station 3A", null, CommentType.String, false, false),
            new(6,  "Station 3B", null, CommentType.String, false, false),
            new(7,  "Station 4", null, CommentType.String, false, false),
            new(8,  "Station 5 (QC)", null, CommentType.String, false, false),
            new(9,  "Wheel Alignment", null, CommentType.String, false, false),
            new(10, "Quality Control", null, CommentType.String, false, false),
            new(11, "Quality Release", null, CommentType.String, false, false),
            new(12, "Dash Remanufacture", null, CommentType.String, false, false),
            new(13, "HVAC", null, CommentType.String, false, false),
            new(14, "Centre Console", null, CommentType.String, false, false),
            new(15, "Seats Conversion", null, CommentType.String, false, false),
            new(16, "Leather Seat Kit", null, CommentType.String, false, false),
            new(17, "Sub Assembly Electrical", null, CommentType.String, false, false),
            new(18, "Invoice", null, CommentType.String, false, false),
            new(19, "Procurement", null, CommentType.String, false, false),
            new(20, "AVV Package", null, CommentType.String, false, false),
            new(21, "Pre-Delivery Inspection", null, CommentType.String, false, false),
            new(22, "Quality", null, CommentType.String, false, false),
        };

    private sealed record ModelTemplate(
        string ModelName,
        string CheckListName,
        int CheckListPosition,
        List<ListItemSeed> Items);

    private sealed record ListItemSeed(
        int Position,
        string Name,
        string? CommentPlaceholder,
        CommentType CommentType,
        bool IsAttachmentRequired,
        bool IsSeparator);
}
