import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { HubConnection, MessagePackHubProtocol, TransportType} from "@aspnet/signalr-client"
//import { HubConnection } from "@aspnet/signalr-client"

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public forecasts: WeatherForecast[];
    public logs: string[] = [];
  
    private connection:HubConnection
    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
       
        this.suscribeOnNonStreamLogMonitor(baseUrl);
      //  this.suscribeOnStreamLogMonitor(baseUrl);
    }

    private suscribeOnNonStreamLogMonitor(baseUrl:string):void
    {
        let msgPackProtocol = new MessagePackHubProtocol();
        //Using MessagePack
        let connection = new HubConnection(baseUrl + '/LogMonitorHub', {
            protocol: msgPackProtocol
            , transport: TransportType.WebSockets
        });

        // let connection = new HubConnection(baseUrl + '/LogMonitorHub');

        connection.on('notify', data => {
            console.log(data);
            this.logs.push(data);
        });

        connection.start().then(() => connection.invoke('Subscribe'));
    }

    private suscribeOnStreamLogMonitor(baseUrl:string): void {

        let msgPackProtocol = new MessagePackHubProtocol();
        //Using MessagePack
        let connection = new HubConnection(baseUrl + '/StreamLogHub');
        
        connection.start().then(() =>
            connection.stream("ObservableCounter", 1, 50).subscribe({
            next: (item:any) => {
                this.logs.push(item);
            },
            error:  (err:any)=> {
               //Log error
            },
            complete: function () {
              //Do something
            }
        }));

       
    }
}





interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
