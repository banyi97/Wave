import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'durationConvert'})
export class DurationConvertPipe implements PipeTransform {
  transform(value: number): string {
    if (typeof (value) != 'number') {
      return "0:00"
    }
    let min = Math.floor(value / 60000);
    let sec = Number(((value % 60000)/1000).toFixed(0));
    if (sec < 10) {
      return `${min}:0${sec}`;
    }
    return `${min}:${sec}`;
  }
}

@Pipe({ name: 'durationSecConvert' })
export class DurationSecConvertPipe implements PipeTransform {
  transform(value: number): string {
    if (typeof (value) != 'number') {
      return "0:00"
    }
    let min = Math.floor(value / 60);
    let sec = Number((value % 60).toFixed(0));
    if (sec < 10) {
      return `${min}:0${sec}`;
    }
    return `${min}:${sec}`;
  }
}