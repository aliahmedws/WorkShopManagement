import { mapEnumToOptions } from '@abp/ng.core';

export enum IssueDeteriorationType {
  NotApplicable = 1,
  Airbags = 2,
  AirbagWarningLamp = 3,
  BrakeFluid = 4,
  BrakeHoses = 5,
  BrakePads = 6,
  BrakeRotors = 7,
  CatalyticConverterExhaust = 8,
  ExhaustMuffler = 9,
  ExhaustTailpipe = 10,
  Windshield = 11,
  Windows = 12,
  CHMSL = 13,
  HeadLamp = 14,
  RegistrationPlateLamp = 15,
  RearRetroReflectors = 16,
  SideIndicatorLamp = 17,
  TailLamp = 18,
  FrontLeftSeatbelt = 19,
  FrontRightSeatbelt = 20,
  RearLeftSeatbelt = 21,
  RearRightSeatbelt = 22,
  TyresFrontAndRear = 23,
  TyresSpare = 24,
}

export const issueDeteriorationTypeOptions = mapEnumToOptions(IssueDeteriorationType);
