import { Directive, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { NgSelectComponent } from '@ng-select/ng-select';
import { Subscription, concat, debounceTime, distinctUntilChanged, of } from 'rxjs';
import { wrapWith } from '../../utilities/observable/observable';
import { EntitySet } from '../../utilities/odata/odata.util';

@Directive({
  selector: '[entitySearch]',
  standalone: true
})
export class EntitySearchDirective<TEntity> implements OnInit, OnDestroy {

  private readonly templateRef: TemplateRef<any>;
  private readonly viewContainerRef: ViewContainerRef;
  private readonly ngSelect: NgSelectComponent;

  @Input({ required: true }) entitySearch!: EntitySet<TEntity>;
  @Input({ required: true }) entitySearchFilter!: (term: string, entitySet: EntitySet<TEntity>) => EntitySet<TEntity>;

  subscriptions: Subscription[] = [];
  httpSubscription?: Subscription;

  constructor(
    templateRef: TemplateRef<any>,
    viewContainerRef: ViewContainerRef,
    ngSelect: NgSelectComponent,
  ) {
    this.templateRef = templateRef;
    this.viewContainerRef = viewContainerRef;
    this.ngSelect = ngSelect;
  }

  ngOnInit(): void {
    const searchSub = concat(of({ term: '' }), this.ngSelect.searchEvent).pipe(
      debounceTime(250),
      distinctUntilChanged(),
    ).subscribe(search => {
      this.httpSubscription?.unsubscribe();
      this.httpSubscription = this.searchEntities(search.term).pipe(
        wrapWith(
          () => {
            this.ngSelect.loading = true;
            this.ngSelect.detectChanges();
          },
          () => {
            this.ngSelect.loading = false;
            this.ngSelect.detectChanges();
          },
        ),
      ).subscribe(records => {
        this.viewContainerRef.clear();
        for (const record of records.value) {
          const context = { $implicit: record };
          this.viewContainerRef.createEmbeddedView(this.templateRef, context);
        }
      });
    });
    this.subscriptions.push(searchSub);
  }

  ngOnDestroy(): void {
    for (const subscription of this.subscriptions) {
      subscription.unsubscribe();
    }
  }

  private searchEntities(term: string) {
    const entitySet = this.entitySearch;
    const filterFn = this.entitySearchFilter;

    const filteredSet = filterFn(term, entitySet);
    return filteredSet.execute();
  }
}
