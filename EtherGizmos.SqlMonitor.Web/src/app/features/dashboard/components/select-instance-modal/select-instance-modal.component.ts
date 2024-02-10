import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { NgbActiveModal, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { Instance } from '../../../../shared/models/instance';
import { IteratePipe } from '../../../../shared/pipes/iterate/iterate.pipe';
import { InstanceService } from '../../../../shared/services/instance/instance.service';
import { Guid } from '../../../../shared/types/guid/guid';

@Component({
  selector: 'app-select-instance-modal',
  standalone: true,
  imports: [
    CommonModule,
    IteratePipe,
    NgbPaginationModule,
  ],
  templateUrl: './select-instance-modal.component.html',
  styleUrl: './select-instance-modal.component.scss'
})
export class SelectInstanceModalComponent implements OnInit {

  $activeModal: NgbActiveModal;

  private $form: FormBuilder;
  private $instance: InstanceService;

  page: number = 1;
  perPage: number = 10;
  isLoading: boolean = false;

  instances: Instance[] = [];
  instanceIdForms: FormControl<Guid>[] = [];
  
  constructor(
    $activeModal: NgbActiveModal,
    $form: FormBuilder,
    $instance: InstanceService,
  ) {
    this.$activeModal = $activeModal;
    this.$form = $form;
    this.$instance = $instance;
  }

  ngOnInit(): void {
    this.searchInstances();
  }

  searchInstances() {
    this.isLoading = true;
    this.$instance.search().subscribe(instances => {
      this.instances = instances;
      this.isLoading = false;
    });
  }

  instanceIsSelected(instance: Instance) {
    return this.instanceIdForms.findIndex(e => e.value === instance.id) >= 0;
  }

  selectInstanceEvent($event: Event, instance: Instance) {
    if ($event.target && ($event.target as HTMLInputElement).checked) {
      this.selectInstance(instance, true);
    } else {
      this.selectInstance(instance, false);
    }
  }

  selectInstance(instance: Instance, selected: boolean) {
    const index = this.instanceIdForms.findIndex(e => e.value === instance.id);
    if (selected === true && index < 0) {
      this.instanceIdForms.push(this.$form.nonNullable.control(instance.id));
    } else if (selected === false && index >= 0) {
      this.instanceIdForms.splice(index, 1);
    }
  }

  trySubmit() {
    this.$activeModal.close(this.instanceIdForms.map(form => form.value));
  }

  stopPropagation($event: MouseEvent) {
    $event.stopPropagation();
  }
}
