import { Directive, ElementRef, OnInit, OnDestroy, Input } from '@angular/core';
import flatpickr from 'flatpickr';
import { Turkish } from 'flatpickr/dist/l10n/tr.js';

@Directive({
  selector: '[appFlatpickr]',
  standalone: true
})
export class FlatpickrDirective implements OnInit, OnDestroy {
  @Input() minDate: any = 'today';
  @Input() enableTime: boolean = false;
  private fpInstance: any;

  constructor(private el: ElementRef) {}

  ngOnInit() {
    this.fpInstance = flatpickr(this.el.nativeElement, {
      locale: Turkish as any,
      dateFormat: this.enableTime ? 'Y-m-d H:i' : 'Y-m-d',
      enableTime: this.enableTime,
      time_24hr: true,
      minDate: this.minDate,
      defaultDate: this.minDate === 'today' ? 'today' : undefined,
      disableMobile: true,
      onReady: (selectedDates, dateStr, instance) => {
        
        if (this.enableTime && instance.timeContainer && instance.calendarContainer) {
          
          instance.timeContainer.style.display = 'none';

          const selectContainer = document.createElement('div');
          selectContainer.style.display = 'flex';
          selectContainer.style.alignItems = 'center';
          selectContainer.style.justifyContent = 'center';
          selectContainer.style.gap = '8px';
          selectContainer.style.padding = '12px';
          selectContainer.style.borderTop = '1px solid #e6e6e6';
          selectContainer.style.backgroundColor = '#f8fafc';

          // --- ÖZEL AÇILIR LİSTE (CUSTOM DROPDOWN) OLUŞTURUCU ---
          const createCustomSelect = (options: string[], defaultVal: string, maxHeight: string) => {
            const wrapper = document.createElement('div');
            wrapper.style.position = 'relative';
            wrapper.style.cursor = 'pointer';

            // Ekranda Görünen Kutu
            const display = document.createElement('div');
            display.innerText = defaultVal;
            display.style.padding = '6px 12px';
            display.style.border = '1px solid #cbd5e1';
            display.style.borderRadius = '6px';
            display.style.backgroundColor = '#fff';
            display.style.width = '65px';
            display.style.textAlign = 'center';
            display.style.fontSize = '14px';

            // Tıklayınca Açılan Kaydırmalı Liste
            const list = document.createElement('div');
            list.style.position = 'absolute';
            list.style.bottom = '100%'; // Takvimin dışına taşmasın diye yukarı doğru açılır
            list.style.left = '0';
            list.style.width = '100%';
            list.style.overflowY = 'auto';
            list.style.backgroundColor = '#fff';
            list.style.border = '1px solid #cbd5e1';
            list.style.borderRadius = '6px';
            list.style.display = 'none';
            list.style.zIndex = '10';
            list.style.boxShadow = '0 -4px 6px -1px rgba(0, 0, 0, 0.1)';
            
            // İŞTE BURASI! AÇILAN LİSTENİN UZUNLUĞUNU BURADAN AYARLAYABİLİRSİN:
            list.style.maxHeight = maxHeight; 

            // Seçenekleri Listeye Ekle
            options.forEach(opt => {
              const item = document.createElement('div');
              item.innerText = opt;
              item.style.padding = '6px';
              item.style.textAlign = 'center';
              item.style.fontSize = '14px';
              item.style.borderBottom = '1px solid #f1f5f9';
              
              item.addEventListener('mouseenter', () => item.style.backgroundColor = '#e2e8f0');
              item.addEventListener('mouseleave', () => item.style.backgroundColor = '#fff');
              
              item.addEventListener('click', (e) => {
                e.stopPropagation();
                display.innerText = opt;
                list.style.display = 'none';
                wrapper.dispatchEvent(new Event('change')); // Değişimi dışarıya bildir
              });
              list.appendChild(item);
            });

            // Tıklayınca Listeyi Aç/Kapat
            display.addEventListener('click', (e) => {
              e.stopPropagation();
              const isOpen = list.style.display === 'block';
              // Varsa diğer açık listeleri kapat (Saat açılırken Dakika kapansın)
              document.querySelectorAll('.custom-fp-list').forEach(el => (el as HTMLElement).style.display = 'none');
              list.style.display = isOpen ? 'none' : 'block';
            });
            list.classList.add('custom-fp-list');

            wrapper.appendChild(display);
            wrapper.appendChild(list);

            return { wrapper, get value() { return display.innerText; } };
          };
          // --------------------------------------------------------

          // Saat ve Dakika Dizilerini Hazırla
          const hours = Array.from({length: 24}, (_, i) => i.toString().padStart(2, '0'));
          const minutes = ['00', '30'];

          // Özel Seçicileri Yarat (Yükseklikleri 150px olarak verdik, buradan değiştirebilirsin)
          const hourSelect = createCustomSelect(hours, '12', '150px');
          const minuteSelect = createCustomSelect(minutes, '00', '100px'); // Dakika listesi kısa olduğu için boyu 100px yeterli

          // Araya İki Nokta (:) Ekle
          const separator = document.createElement('span');
          separator.innerText = ':';
          separator.style.fontWeight = 'bold';
          separator.style.color = '#475569';

          // Değişim Olduğunda Flatpickr'ı Güncelle
          const updateTime = () => {
            const currentDate = instance.selectedDates[0] || new Date(); 
            currentDate.setHours(parseInt(hourSelect.value), parseInt(minuteSelect.value), 0, 0);
            instance.setDate(currentDate, true); 
          };

          hourSelect.wrapper.addEventListener('change', updateTime);
          minuteSelect.wrapper.addEventListener('change', updateTime);

          // Ekranda herhangi bir yere tıklanınca açık kalan listeleri kapatma güvenliği
          document.addEventListener('click', () => {
            document.querySelectorAll('.custom-fp-list').forEach(el => (el as HTMLElement).style.display = 'none');
          });

          // Elementleri sırasıyla kutuya ekle
          selectContainer.appendChild(hourSelect.wrapper);
          selectContainer.appendChild(separator);
          selectContainer.appendChild(minuteSelect.wrapper);
          
          // Kutuyu takvimin en altına monte et
          instance.calendarContainer.appendChild(selectContainer);
        }
      },
      onChange: (selectedDates, dateStr) => {
        this.el.nativeElement.value = dateStr;
        this.el.nativeElement.dispatchEvent(new Event('input'));
      }
    });
  }

  ngOnDestroy() {
    if (this.fpInstance) {
      this.fpInstance.destroy(); 
    }
  }
}