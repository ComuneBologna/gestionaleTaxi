import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    constructor(router: Router) {
        router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                const neEvent = <NavigationEnd>event;
                var noIssUrl = neEvent.url.replace(/iss=[^&\$]*/, '')
                if (neEvent.url != noIssUrl) {
                    router.navigateByUrl(noIssUrl)
                }
            }
        });
    }
}
