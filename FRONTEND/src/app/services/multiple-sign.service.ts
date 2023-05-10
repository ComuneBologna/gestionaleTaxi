import { Injectable } from "@angular/core";
import { TemplateExecutive } from "app/models/licensees.model";

@Injectable()
export class MultipleSignService {
    constructor() {

    }

    public getAll = (): TemplateExecutive[] => {
        let storageItems = window.sessionStorage.getItem("selected");
        let ret: TemplateExecutive[] = [];
        if (storageItems) {
            ret = <TemplateExecutive[]>JSON.parse(storageItems);
        }
        return ret;
    }

    public exists = (id: number): boolean => {
        return this.getAll().findIndex(f => f.id == id) >= 0;
    }


    public add = (item: TemplateExecutive): boolean => {
        let items = this.getAll();
        if (!items.find(f => f.id == item.id)) {
            items.push(item);
            window.sessionStorage.setItem("selected", JSON.stringify(items));
            return true;
        }
        return false;
    }

    public remove = (item: TemplateExecutive): boolean => {
        let items = this.getAll();
        let index = items.findIndex(f => f.id == item.id);
        if (index >= 0) {
            items.splice(index, 1);
            window.sessionStorage.setItem("selected", JSON.stringify(items));
            return true;
        }
        return false;
    }

    public clear = (): void => {
        window.sessionStorage.setItem("selected", JSON.stringify([]));
    }

}