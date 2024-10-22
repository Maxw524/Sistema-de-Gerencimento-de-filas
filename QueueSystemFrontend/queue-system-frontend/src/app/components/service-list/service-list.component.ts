// src/app/components/service-list/service-list.component.ts

import { Component, OnInit } from '@angular/core';
import { QueueService } from '../../services/queue.service';
import { Service } from '../../models/service.model'; // Ajuste conforme o caminho do seu modelo

@Component({
  selector: 'app-service-list',
  templateUrl: './service-list.component.html'
})
export class ServiceListComponent implements OnInit {
  services: Service[] = [];

  constructor(private queueService: QueueService) { }

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.queueService.getServices().subscribe(data => {
      this.services = data;
    });
  }
}
