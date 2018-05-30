
import { Optional } from './../utils/optional';
export class VersionNumber {

    constructor(public major: number, public minor: number, public revision: number, public build: number) { }

    static parse(version: string): VersionNumber {
        let versionComponents = version.split('.');
        let major = Number.parseInt(versionComponents[0]);
        let minor = VersionNumber.extractVersionPart(1, versionComponents);
        let revision = VersionNumber.extractVersionPart(2, versionComponents);
        let build = VersionNumber.extractVersionPart(3, versionComponents);

        return new VersionNumber(major, minor, revision, build);
    }

    static tryParse(version: string) : VersionNumber{
        if(version){
            return VersionNumber.parse(version);
        }
        else{
            return null;
        }
        
    }

    static parseNumber(version: number): VersionNumber{
         const mask: number = 10000;
         
         let build = Math.round((version/mask - Math.trunc(version/mask)) * mask);
         version = Math.trunc(version/mask)
         
         let revision = Math.round((version/mask - Math.trunc(version/mask)) * mask);
         
         version = Math.trunc(version/mask)
         let minor = Math.round((version/mask - Math.trunc(version/mask)) * mask);
         
         version = Math.trunc(version/mask)
         let major = Math.round((version/mask - Math.trunc(version/mask)) * mask);
         
         return new VersionNumber(major, minor, revision, build);
    }
  
    static tryParseNumber(version: number): VersionNumber{
        if(version){
            return VersionNumber.parseNumber(version);
        }
        else{
            return null;
        }
    }

    private static extractVersionPart(index: number, parts: string[]): number {
        if (parts.length < index + 1) {
            return 0;
        }

        return Number.parseInt(parts[index]);
    }

    toString(): string {
        return `${this.major}.${this.minor}.${this.revision}.${this.build}`;
    }

    equals(theOther: VersionNumber): boolean {
        if (!theOther) {
            return false;
        }

        return this.major === theOther.major
            && this.minor === theOther.minor
            && this.revision === theOther.revision
            && this.build === theOther.build;

    }

    toNumber(): number {
        let mask: number = 10000;

        let result: number = this.major;
        result *= mask;
        result += this.minor;
        result *= mask;
        result += this.revision;
        result *= mask;
        result += this.build;

        return result;
    }

}