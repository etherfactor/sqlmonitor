import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NgbActiveModal, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { Instance } from '../../../../shared/models/instance';
import { IteratePipe } from '../../../../shared/pipes/iterate/iterate.pipe';
import { InstanceService } from '../../../../shared/services/instance/instance.service';

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

  trySubmit() {
    this.$activeModal.dismiss();
  }
}
