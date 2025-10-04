import { Directive, ElementRef, OnDestroy, OnInit } from '@angular/core';

@Directive({
  selector: '[appAppendToBody]',
})
export class AppendToBodyDirective implements OnInit, OnDestroy {
  private el: HTMLElement;

  constructor(private elementRef: ElementRef) {
    this.el = this.elementRef.nativeElement;
  }

  ngOnInit() {
    document.body.appendChild(this.el);
  }

  ngOnDestroy() {
    if (this.el && document.body.contains(this.el)) {
      document.body.removeChild(this.el);
    }
  }
}
