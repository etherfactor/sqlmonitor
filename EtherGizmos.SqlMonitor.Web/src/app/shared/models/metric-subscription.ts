import { Observable } from "rxjs";
import { MetricDataService } from "../services/metric-data/metric-data.service";
import { Guid } from "../types/guid/guid";
import { MetricData } from "./metric-data";

export class MetricSubscription {

  private $service: MetricDataService;

  id: Guid;
  metricId: Guid;
  data$: Observable<MetricData>;

  constructor(
    $service: MetricDataService,
    id: Guid,
    metricId: Guid,
    data$: Observable<MetricData>
  ) {
    this.$service = $service;

    this.id = id;
    this.metricId = metricId;
    this.data$ = data$;
  }

  unsubscribe() {

  }
}
