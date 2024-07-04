import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Observable, catchError, throwError } from "rxjs";
import { ZodType } from "zod";
import { BodyContainerType, BodyService } from "../../../services/body/body.service";
import { NavbarMenuAction, NavbarMenuBreadcrumb, NavbarMenuService } from "../../../services/navbar-menu/navbar-menu.service";
import { Bound } from "../../../utilities/bound/bound.util";
import { DefaultControlTypes, TypedFormGroup } from "../../../utilities/form/form.util";

@Component({
  selector: 'app-editable',
  template: ''
})
export abstract class EditableComponent<TEntity, TKey> implements OnInit {

  private readonly $activatedRoute: ActivatedRoute;
  private readonly $body: BodyService;
  private readonly $navbarMenu: NavbarMenuService;
  private readonly keyParse: ZodType;

  id?: TKey;
  isLoading: boolean = true;
  isEditing: boolean = false;
  isNew: boolean = true;

  entity: TEntity;
  form?: TypedFormGroup<TEntity, DefaultControlTypes>;

  constructor(
    $activatedRoute: ActivatedRoute,
    $body: BodyService,
    $navbarMenu: NavbarMenuService,
    keyParse: ZodType,
  ) {
    this.$activatedRoute = $activatedRoute;
    this.$body = $body;
    this.$navbarMenu = $navbarMenu;
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

  protected abstract loadForm(entity: TEntity): TypedFormGroup<TEntity, DefaultControlTypes>;

  ngOnInit(): void {
    this.$body.setContainer(BodyContainerType.Normal);

    const testId = this.$activatedRoute.snapshot.paramMap.get('id') as TKey;
    if (testId) {
      this.keyParse.parse(testId);
      this.id = testId;

      this.isLoading = true;
      this.isEditing = false;
      this.isNew = false;
    } else {
      this.id = undefined;

      this.isLoading = false;
      this.isEditing = true;
      this.isNew = true;
    }

    this.initialize();
  }

  private initialize(): void {
    this.refresh();

    if (this.id) {
      this.isLoading = true;
      this.loadRecord(this.id).pipe(
        catchError(err => {
          this.isLoading = false;
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.initializeForm(entity);
        this.refresh();
        this.isLoading = false;
      });
    } else {
      this.initializeForm({} as TEntity);
      this.refresh();
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
    console.log(form);
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
    this.refresh();
  }

  @Bound save(): void {
    this.isEditing = false;
    //do save
  }
}
