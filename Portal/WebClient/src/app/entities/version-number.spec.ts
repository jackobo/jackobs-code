import { VersionNumber } from './version-number';
import { async } from '@angular/core/testing';
describe('VersionNumber.parse', () => {


   it('major should be set to corect value', async(() => {
    let version = VersionNumber.parse("1.2.3.4");
    expect(version.major).toEqual(2);
  }));

  it('minor should be set to corect value', async(() => {
    let version = VersionNumber.parse("1.2.3.4");
    expect(version.minor).toEqual(2);
  }));

  it('revision should be set to corect value', async(() => {
    let version = VersionNumber.parse("1.2.3.4");
    expect(version.revision).toEqual(3);
  }));

  it('build should be set to corect value', async(() => {
    let version = VersionNumber.parse("1.2.3.4");
    expect(version.build).toEqual(4);
  }));

  it('build should default to zero if missing', async(() => {
    let version = VersionNumber.parse("1.2.3");
    expect(version.build).toEqual(0);
  }));

  it('revision should default to zero if missing', async(() => {
    let version = VersionNumber.parse("1.2");
    expect(version.revision).toEqual(0);
  }));

   it('minor should default to zero if missing', async(() => {
    let version = VersionNumber.parse("1");
    expect(version.minor).toEqual(0);
  }));



});

describe('VersionNumber.toString', () => {
  it('should return the version in the correrct format', async(() => {
    let version = new VersionNumber(1,2,3,4);
    expect(version.toString()).toEqual("1.2.3.4");
  }));
});

describe('VersionNumber.equals', () => {
  it('should retrun true if all properties are equal', async(() => {
    let v1 = new VersionNumber(1,2,3,4);
    let v2 = new VersionNumber(1,2,3,4);
    expect(v1.equals(v2)).toBeTruthy();
  }));


  it('should retrun false if all properties are equal', async(() => {
    let v1 = new VersionNumber(1,2,3,4);
    let v2 = new VersionNumber(4,3,2,1);
    expect(v1.equals(v2)).toBeFalsy();
  }));
});

describe('VersionNumber.toNumber', () => {
  it('should return the correct computed number', async(() => {
    let version = new VersionNumber(1,2,3,4);
    expect(version.toNumber()).toEqual(1000200030004);
  }));
});

describe('VersionNumber.parseNumber', () => {
  it('should return 0.0.0.0 if argument is 0', async(() => {
    let version = VersionNumber.parseNumber(0);
    expect(version.toString()).toEqual("0.0.0.0");
  }));

   it('should return 0.0.0.1 if argument is 1', async(() => {
    let version = VersionNumber.parseNumber(1);
    expect(version.toString()).toEqual("0.0.0.1");
  }));

  it('should return 0.0.2.1 if argument is 20001', async(() => {
    let version = VersionNumber.parseNumber(20001);
    expect(version.toString()).toEqual("0.0.2.1");
  }));

  it('should return 0.0.1.0 if argument is 10000', async(() => {
    let version = VersionNumber.parseNumber(10000);
    expect(version.toString()).toEqual("0.0.1.0");
  }));

  it('should return 1.0.0.0 if argument is 1000000000000', async(() => {
    let version = VersionNumber.parseNumber(1000000000000);
    expect(version.toString()).toEqual("1.0.0.0");
  }));

  it('should return 5.6.3.4 if argument is 5000600030004', async(() => {
    let version = VersionNumber.parseNumber(5000600030004);
    expect(version.toString()).toEqual("5.6.3.4");
  }));

  it('should return 1.2.3.4 if argument is 1000200030004', async(() => {
    let version = VersionNumber.parseNumber(1000200030004);
    expect(version.toString()).toEqual("1.2.3.4");
  }));


});

