import { Observable } from "rxjs";
import { Guid } from "../types/guid/guid";
import { MetricData } from "./metric-data";

export class MetricSubscription {

  id: Guid;
  data$: Observable<MetricData>;

  constructor(id: Guid, data$: Observable<MetricData>) {
    this.id = id;
    this.data$ = data$;
  }
}
