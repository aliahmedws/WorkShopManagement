/**
 * Normalizes a label/placeholder into an ABP localization key.
 * - Trims whitespace
 * - Returns '' for null/undefined/empty
 * - Adds '::' prefix if missing
 */
export function localizeKey(input: unknown): string {
  const t = (input ?? '').toString().trim();
  if (!t) return '';
  return t.startsWith('::') ? t : `::${t}`;
}