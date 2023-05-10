import { SearchCriteria } from "@asf/ng14-library";

export abstract class ShiftBase {
    public name: string;
    public durationInHour: number;

}

export class ShiftItem extends ShiftBase {
    public id: number;
}

export class ShiftToWrite extends ShiftBase {
    public isHandicapMode: boolean = false;
    public handicapBeforeInHour: number;
    public handicapAfterInHour: number;
    public breakInHours: number;
    public breakThresholdActivationInHour: number;
    public restDayFrequencyInDays: number;
    public subShifts: SubShift[] = [];
}

export class Shift extends ShiftToWrite {
    public id: number;
}


export class SubShift {
    public id: number = 0;
    public name: string;
    public restDay: DaysOfWeek;
}

export class ShiftSmall {
    public id: number;
    public name: string;
}

export class SubShiftSmall {
    public id: number;
    public shiftId: number;
    public name: string;
}

export enum DaysOfWeek {
    Sunday = "Sunday",
    Monday = "Monday",
    Tuesday = "Tuesday",
    Wednesday = "Wednesday",
    Thursday = "Thursday",
    Friday = "Friday",
    Saturday = "Saturday"
}

export class ShiftsSearchCriteria extends SearchCriteria {
    public id: number;
    public name: string;
    public durationInHour: number;
}