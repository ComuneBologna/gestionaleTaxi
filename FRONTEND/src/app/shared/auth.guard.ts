import { Injectable } from '@angular/core';
import { BaseAuthGuard, AuthenticationService } from '@asf/ng14-library';

@Injectable()
export class AuthGuard extends BaseAuthGuard {

    constructor(authService: AuthenticationService) {
        super(authService)
    }
}