import { ValidatorFn } from "@angular/forms";
import { DateTime, DateTimeUnit } from "luxon";
import { z } from "zod";

export type RelativeTime = string & { __relativeTimeTag: true };

export enum RelativeTimeUnitType {
  Year = "Y",
  Month = "M",
  Week = "w",
  Day = "d",
  Hour = "h",
  Minute = "m",
  Second = "s",
}

export enum RelativeTimeRoundType {
  Start = "s",
  End = "e",
}

export interface RelativeTimeInterpretation {
  offsetUnit?: RelativeTimeUnitType;
  offsetValue?: number;

  roundType?: RelativeTimeRoundType;
  roundUnit?: RelativeTimeUnitType;

  friendlyText: string;
}

const regex = /^\s*?t(?:\s*?(?<ASIGN>[+-])\s*?(?<AVALUE>\d+)\s*?(?<AUNIT>Y|M|w|d|h|m|s))?(?:\s*?@\s*?(?<RTYPE>[SsEe])[Oo](?<RUNIT>Y|M|w|d|h|m|s))?\s*?$/;
const timeAbbreviations: { [key: string]: string } = {
  [RelativeTimeUnitType.Year]: 'year',
  [RelativeTimeUnitType.Month]: 'month',
  [RelativeTimeUnitType.Week]: 'week',
  [RelativeTimeUnitType.Day]: 'day',
  [RelativeTimeUnitType.Hour]: 'hour',
  [RelativeTimeUnitType.Minute]: 'minute',
  [RelativeTimeUnitType.Second]: 'second',
};

export function isRelativeTime(value: unknown): value is RelativeTime {
  if (typeof value === 'string') {
    return regex.test(value);
  }

  return false;
}

export function parseRelativeTime(value: unknown): RelativeTime {
  if (isRelativeTime(value)) {
    return value;
  }

  throw new Error(`Value ${value} is not a valid relative time.`);
}

export const RelativeTimeZ = z.custom<RelativeTime>(value => {
  return isRelativeTime(value);
});

export function interpretRelativeTime(value: RelativeTime): RelativeTimeInterpretation {

  const regexResult = regex.exec(value);

  if (!regexResult || !regexResult.groups)
    throw new Error();

  let textValue = 'now';

  const addSign = regexResult.groups['ASIGN']
    ? regexResult.groups['ASIGN']
    : undefined;

  const addValue = regexResult.groups['AVALUE']
    ? parseInt(regexResult.groups['AVALUE'])
    : undefined;

  const addUnit = regexResult.groups['AUNIT']
    ? regexResult.groups['AUNIT'] as RelativeTimeUnitType
    : undefined;

  if (addSign && addValue && addUnit) {
    textValue = `${addValue} ${timeAbbreviations[addUnit]}${addValue !== 1 ? 's' : ''}`;
    textValue = `${textValue} ${addSign === '+' ? 'from now' : 'ago'}`;
  }

  const roundType = regexResult.groups['RTYPE']
    ? (regexResult.groups['RTYPE'].toLowerCase() === 's' ? RelativeTimeRoundType.Start : RelativeTimeRoundType.End)
    : undefined;

  const roundUnit = regexResult.groups['RUNIT']
    ? regexResult.groups['RUNIT'] as RelativeTimeUnitType
    : undefined;

  if (roundType && roundUnit) {
    if (addSign && addValue && addUnit) {
      textValue = `the ${roundType === RelativeTimeRoundType.Start ? 'start' : 'end'} of the ${timeAbbreviations[roundUnit]} ${textValue}`;
    } else {
      textValue = `the ${roundType === RelativeTimeRoundType.Start ? 'start' : 'end'} of the current ${timeAbbreviations[roundUnit]}`;
    }
  }

  const result = {
    offsetUnit: addUnit,
    offsetValue: addValue ? ((addSign === '+' ? 1 : -1) * addValue) : undefined,

    roundType: roundType,
    roundUnit: roundUnit,

    friendlyText: textValue,
  };
  return result;
}

export const isRelativeTimeValidator: ValidatorFn = control => {
  if (!control.value)
    return null;

  if (!isRelativeTime(control.value)) {
    return { notRelativeTime: true };
  }

  return null;
};

export function evaluateRelativeTime(interpretation: RelativeTimeInterpretation): DateTime {
  let result = DateTime.now();

  if (interpretation.offsetUnit && interpretation.offsetValue) {
    switch (interpretation.offsetUnit) {
      case (RelativeTimeUnitType.Year):
        result = result.plus({ years: interpretation.offsetValue });
        break;

      case (RelativeTimeUnitType.Month):
        result = result.plus({ months: interpretation.offsetValue });
        break;

      case (RelativeTimeUnitType.Week):
        result = result.plus({ weeks: interpretation.offsetValue });
        break;

      case (RelativeTimeUnitType.Day):
        result = result.plus({ days: interpretation.offsetValue });
        break;

      case (RelativeTimeUnitType.Hour):
        result = result.plus({ hours: interpretation.offsetValue });
        break;

      case (RelativeTimeUnitType.Minute):
        result = result.plus({ minutes: interpretation.offsetValue });
        break;

      case (RelativeTimeUnitType.Second):
        result = result.plus({ seconds: interpretation.offsetValue });
        break;
    }
  }

  if (interpretation.roundType && interpretation.roundUnit) {
    let roundTo: DateTimeUnit;
    switch (interpretation.roundUnit) {
      case (RelativeTimeUnitType.Year):
        roundTo = 'year';
        break;

      case (RelativeTimeUnitType.Month):
        roundTo = 'month';
        break;

      case (RelativeTimeUnitType.Week):
        roundTo = 'week';
        break;

      case (RelativeTimeUnitType.Day):
        roundTo = 'day';
        break;

      case (RelativeTimeUnitType.Hour):
        roundTo = 'hour';
        break;

      case (RelativeTimeUnitType.Minute):
        roundTo = 'minute';
        break;

      case (RelativeTimeUnitType.Second):
        roundTo = 'second';
        break;
    }

    if (interpretation.roundType === RelativeTimeRoundType.Start) {
      result = result.startOf(roundTo);
    } else {
      result = result.endOf(roundTo);
    }
  }

  return result;
}

export function getTimeRangeText(startInterpretation: RelativeTimeInterpretation, endInterpretation: RelativeTimeInterpretation) {
  if (isOffset(endInterpretation) && isRound(endInterpretation)) {
    //Do nothing
  } else if (isOffset(endInterpretation)) {
    //Do nothing
  } else if (isRound(endInterpretation)) {
    if (isOffset(startInterpretation) && isRound(endInterpretation)) {
      //Do nothing
    } else if (isOffset(startInterpretation)) {
      //Do nothing
    } else if (isRound(startInterpretation)) {
      if (startInterpretation.roundUnit === endInterpretation.roundUnit) {
        return `This ${timeAbbreviations[startInterpretation.roundUnit]} in its entirety`;
      } else {
        //Do nothing
      }
    } else {
      //Do nothing
    }
  } else {
    if (isOffset(startInterpretation) && isRound(startInterpretation)) {
      if (startInterpretation.roundType === RelativeTimeRoundType.Start) {
        return `Start of ${-startInterpretation.offsetValue} ${timeAbbreviations[startInterpretation.offsetUnit]}${startInterpretation.offsetValue !== 1 ? 's' : ''} ago onward`;
      } else {
        return `End of ${-startInterpretation.offsetValue} ${timeAbbreviations[startInterpretation.offsetUnit]}${startInterpretation.offsetValue !== 1 ? 's' : ''} ago onward`;
      }
    } else if (isOffset(startInterpretation)) {
      return `Last ${-startInterpretation.offsetValue} ${timeAbbreviations[startInterpretation.offsetUnit]}${startInterpretation.offsetValue !== 1 ? 's' : ''}`;
    } else if (isRound(startInterpretation)) {
      return `This ${timeAbbreviations[startInterpretation.roundUnit]} so far`;
    } else {
      return 'Nothing';
    }
  }

  const startTime = evaluateRelativeTime(startInterpretation);
  const endTime = evaluateRelativeTime(endInterpretation)
  return `${startTime.toLocaleString({ dateStyle: "short", timeStyle: "short" })} to ${endTime.toLocaleString({ dateStyle: "short", timeStyle: "short" })}`;
}

function isNow(interpretation: RelativeTimeInterpretation) {
  return !isOffset(interpretation) && !isRound(interpretation);
}

function isOffset(interpretation: RelativeTimeInterpretation): interpretation is RelativeTimeInterpretation & Required<Pick<RelativeTimeInterpretation, 'offsetUnit' | 'offsetValue'>> {
  return interpretation.offsetUnit !== undefined
    && interpretation.offsetUnit !== null
    && interpretation.offsetValue !== undefined
    && interpretation.offsetValue !== null;
}

function isRound(interpretation: RelativeTimeInterpretation): interpretation is RelativeTimeInterpretation & Required<Pick<RelativeTimeInterpretation, 'roundType' | 'roundUnit'>> {
  return interpretation.roundType !== undefined
    && interpretation.roundType !== null
    && interpretation.roundUnit !== undefined
    && interpretation.roundUnit !== null;
}
