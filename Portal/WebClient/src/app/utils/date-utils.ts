export class DateUtils{
    static parse(date: string) : Date{
        let x: string = date.replace('/Date(','');
        x = x.substr(0, x.indexOf('+'));
        return new Date(parseInt(x));
    }
}