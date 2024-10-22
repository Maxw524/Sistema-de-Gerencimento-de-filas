import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http'; // Importar HttpClientModule
import { AppComponent } from './app.component';
import { QueueService } from './services/queue.service'; // Ajuste o caminho conforme necessário
@NgModule({
  declarations: [
    AppComponent,
    ServiceListComponent, // Adicione aqui
    // outros componentes...
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [QueueService],
  bootstrap: [AppComponent]
})
export class AppModule { }
import { ServiceListComponent } from './components/service-list/service-list.component'; // Ajuste o caminho se necessário
