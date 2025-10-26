![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Line Coverage](badges/line-coverage.svg)
![Branch Coverage](badges/branch-coverage.svg)

# st-test-doubles-csharp  
# Menulis Unit Test dengan Test Doubles (C# + NUnit + Moq)

---

## Deskripsi Singkat
Repositori ini merupakan template tugas untuk latihan menulis **unit test** menggunakan berbagai jenis **Test Doubles** (Dummy, Stub, Fake, Spy, dan Mock) pada bahasa pemrograman **C#** dengan framework **NUnit** dan library **Moq**.  
Mahasiswa diminta menulis unit test di folder `tests/` untuk menguji service yang sudah tersedia di folder `src/`.

---

## Capaian Pembelajaran
Setelah menyelesaikan tugas ini, mahasiswa mampu:
1. Menjelaskan konsep dan tujuan penggunaan **Test Doubles** dalam pengujian unit.  
2. Membedakan jenis-jenis Test Double (Dummy, Stub, Fake, Spy, Mock) beserta fungsinya.  
3. Menggunakan **Moq** untuk membuat dan memverifikasi Mock Object.  
4. Mengimplementasikan pengujian **state-based** dan **behavior-based** secara tepat.  
5. Menulis unit test dengan cakupan minimal **80% line coverage** dan **70% branch coverage**.  
6. Menggunakan pipeline **GitHub Actions** untuk otomatisasi pengujian dan pelaporan coverage.

---

## Target
- Menulis unit test yang mengisolasi SUT menggunakan **Dummy, Stub, Fake, Spy, Mock**.
- Mencapai **line coverage ≥ 80%** dan **branch coverage ≥ 70%** (diperiksa CI).
- Memahami kapan memakai Stub/Fake (state-based) vs Spy/Mock (behavior-based).

---

## Struktur
- `src/Core/` → kode produksi: Interfaces, Models, Services (jangan diubah)  
- `tests/Core.Tests/` → tulis semua unit test di sini  

---

## Lingkungan Pengembangan
- **SDK:** .NET 8.0 (LTS)  
- **Target Framework:** net8.0  
- **IDE:** Visual Studio / VS Code  
- **Struktur:** standar Visual Studio (`src/` dan `tests/`)  
- **Perintah utama:**
  ```bash
  dotnet restore
  dotnet build
  dotnet test
  ````

---

## Konteks dan Use Case

### PriceService – menggunakan IExchangeRate

Layanan ini bertugas mengonversi harga barang dari mata uang USD ke IDR dengan memanfaatkan dependensi `IExchangeRate`. Dalam kondisi nyata, service ini mungkin memanggil API kurs eksternal. Namun, untuk pengujian unit, kamu akan mengganti dependensi tersebut dengan **Stub** yang selalu mengembalikan nilai kurs tertentu, misalnya 1 USD = 15.000 IDR. Dengan cara ini, hasil konversi menjadi deterministik dan mudah diverifikasi tanpa bergantung pada sumber data eksternal.

### RegistrationService – menggunakan IUserRepository

Layanan ini bertanggung jawab untuk mendaftarkan user baru ke dalam sistem. Ia menggunakan `IUserRepository` untuk menambah dan mencari data user. Dalam implementasi sebenarnya, repository mungkin terhubung dengan database. Untuk keperluan pengujian, kamu akan membuat **Fake Repository** sederhana berbasis in-memory yang dapat menyimpan dan mencari user tanpa perlu koneksi ke database sungguhan. Dengan pendekatan ini, kamu dapat menguji logika pendaftaran—misalnya, menolak username yang sudah terdaftar—secara cepat dan terisolasi.

### OrderService – menggunakan IEmailService, IPaymentGateway, dan ILogger

Layanan ini menangani proses pemesanan barang. Saat pesanan dibuat, service akan mencoba melakukan pembayaran melalui `IPaymentGateway`. Jika pembayaran berhasil, service mengirim email konfirmasi kepada pelanggan melalui `IEmailService`. Jika pembayaran gagal, ia akan mencatat pesan kesalahan menggunakan `ILogger`. Dalam pengujian, kamu akan menggunakan **Mock (Moq)** untuk memverifikasi bahwa `IEmailService` dan `IPaymentGateway` dipanggil dengan benar, serta **Spy** untuk memastikan `ILogger` benar-benar mencatat pesan saat transaksi gagal. Tujuannya agar pengujian tidak hanya memeriksa hasil akhir, tetapi juga memastikan interaksi antar komponen terjadi sebagaimana mestinya.

---

## Skenario Pengujian

### PriceService – menggunakan IExchangeRate

Service ini bertugas mengonversi nilai mata uang dari USD ke IDR.
Dalam pengujian, kamu akan membuat **Stub** untuk `IExchangeRate` agar selalu mengembalikan kurs tertentu.
Gunakan skenario berikut sebagai dasar penulisan test:

1. **Kurs tetap**
   Jika kurs USD→IDR = 15.000 dan input = 10 USD, hasil konversi harus 150.000 IDR.
   *Gunakan Stub IExchangeRate dengan nilai tetap.*

2. **Kurs ekstrem**
   Jika kurs = 0, hasil konversi juga harus 0. *Pastikan service tidak melempar exception.*

3. **Nilai desimal**
   Jika kurs = 15.250 dan input = 2.5 USD, hasil harus 38.125 IDR. *Uji perhitungan desimal dan presisi.*

---

### RegistrationService – menggunakan IUserRepository

Service ini mendaftarkan user baru dan menolak username yang sudah ada.
Dalam pengujian, kamu akan membuat **Fake Repository (in-memory)** untuk menyimpan data user.

1. **Pendaftaran baru berhasil**
   Jika repository kosong dan user baru didaftarkan, service harus mengembalikan `true`. *Gunakan Fake repository dan periksa bahwa user tersimpan.*

2. **Duplikasi username ditolak**
   Jika username sudah ada, pendaftaran berikutnya harus mengembalikan `false`. *Gunakan Fake repository yang menyimpan state.*

3. **Konsistensi data**
   Setelah dua user berbeda didaftarkan, repository harus menyimpan keduanya. *Periksa jumlah item di Fake repository.*

---

### OrderService – menggunakan IEmailService, IPaymentGateway, dan ILogger

Service ini menangani proses pemesanan dan mengandalkan beberapa dependensi.
Dalam pengujian, kamu akan mengombinasikan **Mock (Moq)** dan **Spy**.

1. **Pembayaran berhasil → kirim email konfirmasi**
   Jika `IPaymentGateway.Charge()` mengembalikan `true`, service harus:

   * Memanggil `IEmailService.SendEmail()` tepat **satu kali**.
   * Tidak memanggil `ILogger.Log()`. 
   * *Gunakan Mock untuk memverifikasi `SendEmail`.*

2. **Pembayaran gagal → catat error**
   Jika `IPaymentGateway.Charge()` mengembalikan `false`, service harus:

   * Tidak mengirim email sama sekali.
   * Memanggil `ILogger.Log()` minimal **sekali** dengan pesan yang mengandung “failed”.
   * *Gunakan Spy untuk memverifikasi logging.*

3. **Validasi interaksi berurutan (opsional)**
   Uji bahwa pembayaran terjadi **sebelum** pengiriman email.

*Tugas Anda adalah menerjemahkan setiap skenario di atas menjadi unit test nyata di `tests/Core.Tests/` dengan memanfaatkan Test Doubles (Stub, Fake, Mock, Spy) yang sesuai.*

---

## Petunjuk Pengerjaan

1. **Baca kode sumber** di folder `src/Core/`.
   Di folder tersebut terdapat tiga layanan utama yang akan diuji:

   * `PriceService` – menggunakan `IExchangeRate`
   * `RegistrationService` – menggunakan `IUserRepository`
   * `OrderService` – menggunakan `IEmailService`, `IPaymentGateway`, dan `ILogger`

2. **Tulis unit test** di folder `tests/Core.Tests/`.
   Buat file test baru (atau lengkapi file yang ada) untuk setiap layanan:

   * Gunakan **Stub** untuk menguji `PriceService`
   * Gunakan **Fake (in-memory repository)** untuk `RegistrationService`
   * Gunakan **Mock (Moq)** dan **Spy** untuk `OrderService`

3. **Jalankan test lokal**

   ```bash
   dotnet restore
   dotnet test
   ```

4. **Pastikan semua test lulus sebelum di-push ke GitHub.**

5. **Push hasil ke GitHub.**
   Setelah push, **GitHub Actions** akan otomatis:

   * Menjalankan test
   * Menghitung **line & branch coverage**
   * Memperbarui badge coverage di README
   * Gagal jika coverage < 80% (line) atau < 70% (branch)

6. **Tujuan akhir**

   * Semua test lulus ✅
   * Coverage badge berwarna **hijau** 🟢
   * Tidak ada modifikasi di folder `src/`

---

> ⚠️ **Penting:** Anda **hanya** menulis/menambah kode di folder `tests/`. Folder `src/` adalah konteks sistem yang diuji dan **tidak boleh diubah**.

---

=== SELESAI ===
