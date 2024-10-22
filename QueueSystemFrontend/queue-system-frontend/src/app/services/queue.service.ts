// src/app/services/queue.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Service } from '../models/service.model'; // Certifique-se de que este caminho está correto
import { Ticket } from '../models/ticket.model'; // Certifique-se de que este caminho está correto

@Injectable({
  providedIn: 'root'
})
export class QueueService {
  private apiUrl = 'https://localhost:5001'; // Ajuste a URL conforme necessário

  constructor(private http: HttpClient) { }

  getServices(): Observable<Service[]> {
    return this.http.get<Service[]>(`${this.apiUrl}/services`);
  }

  createService(service: Service): Observable<Service> {
    return this.http.post<Service>(`${this.apiUrl}/services`, service);
  }

  // Adicione métodos para tickets...
}
