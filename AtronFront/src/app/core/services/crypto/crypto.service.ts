import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  // A mesma chave utilizada no back-end (AES-256)
  private readonly secretKey = 'AtronTrackerSecretKeyAES256Bits!';

  constructor() { }

  encrypt(value: string): string {
    if (!value) return '';
    const encrypted = CryptoJS.AES.encrypt(value, this.secretKey);
    return encrypted.toString();
  }

  decrypt(valueToDecrypt: string): string {
    if (!valueToDecrypt) return '';
    try {
      const decrypted = CryptoJS.AES.decrypt(valueToDecrypt, this.secretKey);
      return decrypted.toString(CryptoJS.enc.Utf8);
    } catch {
      return '';
    }
  }
}
