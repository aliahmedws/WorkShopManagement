import { ENTITY_TYPE_MAP } from "./entity-type.constants";

export function resolveEntityTypeFullName(entityName: string | null): string {
    const key = entityName?.trim().toLowerCase();
    if (!key) return '';

    const ns = ENTITY_TYPE_MAP[key];
    return ns ? `WorkShopManagement.${ns}` : '';
}