import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { BootstrapUtils } from '@asf/ng14-library';
import { environment } from 'environments/environment';
import { AppModule } from './app/app.module';

BootstrapUtils.boot(environment).subscribe(() => {
    platformBrowserDynamic().bootstrapModule(AppModule)
        .catch(err => console.error(err));
});
