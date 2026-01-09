import { mapEnumToOptions } from '@abp/ng.core';

export enum ChoiceOptions {
  Yes = 1,
  No = 2,
}

export const choiceOptionsOptions = mapEnumToOptions(ChoiceOptions);
