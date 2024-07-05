import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, catchError, throwError } from "rxjs";
import { ZodType } from "zod";
import { BodyContainerType, BodyService } from "../../../services/body/body.service";
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from "../../../services/navbar-menu/navbar-menu.service";
import { Bound } from "../../../utilities/bound/bound.util";
import { DefaultControlTypes, TypedFormGroup, getDirtyFormValues } from "../../../utilities/form/form.util";

@Component({
  selector: 'app-editable',
  template: ''
})
export abstract class EditableComponent<TEntity, TKey> implements OnInit {

  protected readonly $activatedRoute: ActivatedRoute;
  protected readonly $body: BodyService;
  protected readonly $navbarMenu: NavbarMenuService;
  protected readonly $router: Router;
  protected readonly keyParse: ZodType;

  id?: TKey;

  get isLoading(): boolean { return this.isLoadingStack > 0; }
  private isLoadingStack: number = 0;
  isEditing: boolean = false;
  isNew: boolean = true;

  entity: TEntity;
  form?: TypedFormGroup<TEntity, DefaultControlTypes>;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $navbarMenu: NavbarMenuService,
    $router: Router,
    keyParse: ZodType,
  ) {
    this.$activatedRoute = $activatedRoute;
    this.$body = $body;
    this.$navbarMenu = $navbarMenu;
    this.$router = $router;
    this.keyParse = keyParse;

    this.entity = {} as TEntity;
  }

  private refresh(): void {
    this.updateBreadcrumbs();
    this.updateActions();
  }

  protected abstract get actions(): NavbarMenuAction[];

  private updateActions() {
    this.$navbarMenu.setActions(this.actions);
  }

  protected abstract get breadcrumbs(): NavbarMenuBreadcrumb[];

  private updateBreadcrumbs() {
    this.$navbarMenu.setBreadcrumbs(this.breadcrumbs);
  }

  protected abstract loadRecord(key: TKey): Observable<TEntity>

  protected abstract createEmptyRecord(): TEntity;

  protected abstract loadForm(entity: TEntity): TypedFormGroup<TEntity, DefaultControlTypes>;

  protected abstract createRecord(entity: Partial<TEntity>): Observable<TEntity>;

  protected abstract updateRecord(key: TKey, entity: Partial<TEntity>): Observable<TEntity>;

  protected abstract navigateToRecord(entity: TEntity): void;

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);

    const testId = this.$activatedRoute.snapshot.paramMap.get('id') as TKey;
    if (testId) {
      this.keyParse.parse(testId);
      this.id = testId;

      this.isEditing = false;
      this.isNew = false;
    } else {
      this.id = undefined;

      this.isEditing = true;
      this.isNew = true;
    }

    this.initialize();
  }

  private initialize(): void {
    this.refresh();

    if (this.id) {
      this.isLoadingStack++;
      this.loadRecord(this.id).pipe(
        catchError(err => {
          this.isLoadingStack--;
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.initializeForm(entity);
        this.isLoadingStack--;
      });
    } else {
      const entity = this.createEmptyRecord();
      this.initializeForm(entity);
    }
  }

  private initializeForm(entity: TEntity): void {
    this.entity = entity;

    const form = this.loadForm(entity);

    if (this.isEditing) {
      form.enable();
    } else {
      form.disable();
    }

    this.form = form;

    this.refresh();
  }

  @Bound edit(): void {
    this.isEditing = true;
    this.form?.enable();

    this.refresh();
  }

  @Bound cancel(): void {
    this.isEditing = false;
    this.form?.disable();

    this.initializeForm(this.entity);
  }

  @Bound save(): void {
    this.isEditing = false;

    if (!this.form)
      return;

    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const data = getDirtyFormValues(this.form);

    if (this.isNew) {
      this.isLoadingStack++;
      this.createRecord(data).pipe(
        catchError(err => {
          this.isLoadingStack--;
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.navigateToRecord(entity);
      });
    } else {
      if (!this.id)
        return;

      this.isLoadingStack++;
      this.updateRecord(this.id, data).pipe(
        catchError(err => {
          this.isLoadingStack--;
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.isLoadingStack--;
        this.initializeForm(entity);
        this.refresh();
      });
    }
  }
}
