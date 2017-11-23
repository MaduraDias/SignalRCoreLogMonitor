import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { HubConnection, MessagePackHubProtocol} from "@aspnet/signalr-client"
//import { HubConnection } from "@aspnet/signalr-client"

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public forecasts: WeatherForecast[];
    public logs: string[]=[];
   
    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
         let msgPackProtocol = new MessagePackHubProtocol();
       
        //Using MessagePack
        let connection = new HubConnection(baseUrl + '/SniffHub', { protocol: msgPackProtocol });
        
       // let connection = new HubConnection(baseUrl + '/SniffHub');
                
        connection.on('notify', data => {
            console.log(data);
            this.logs.push(data);
        });
        
    
        connection.start().then(() => connection.invoke('subscribe'));
    }
}




interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
