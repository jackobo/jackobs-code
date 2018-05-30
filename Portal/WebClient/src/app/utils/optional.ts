export class Optional<T>{
   
    content : T[] = [];

    static some<Z>(item : Z) : Optional<Z>{
        var opt = new Optional<Z>();
        opt.content = [item];
        return opt;
    }

    static none<Z>() : Optional<Z>{
        return new Optional<Z>();
    }

    do(callBack: (item : T) => void){
        for(let i of this.content){
            callBack(i);
        }
    }

    any() : boolean{
        return this.content.length > 0;
    }
}