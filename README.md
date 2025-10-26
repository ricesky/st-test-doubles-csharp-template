![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Line Coverage](badges/line-coverage.svg)
![Branch Coverage](badges/branch-coverage.svg)

# st-test-doubles-csharp

# Menulis Unit Test dengan Test Doubles (C# + NUnit + Moq)

---

## Target
- Menulis unit test yang mengisolasi SUT menggunakan **Dummy, Stub, Fake, Spy, Mock**.
- Mencapai **line coverage ≥ 80%** dan **branch coverage ≥ 70%** (diperiksa CI).
- Memahami kapan memakai Stub/Fake (state-based) vs Spy/Mock (behavior-based).

---

## Struktur
- src/Core/ -> kode produksi: Interfaces, Models, Services (jangan diubah)
- tests/Core.Tests/ -> tulis semua unit test di sini

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

---

## Petunjuk Pengerjaan

1. **Baca kode sumber** di folder `src/Core/`  
   Di folder tersebut terdapat tiga layanan utama yang akan diuji:
   - `PriceService` – menggunakan `IExchangeRate`
   - `RegistrationService` – menggunakan `IUserRepository`
   - `OrderService` – menggunakan `IEmailService`, `IPaymentGateway`, dan `ILogger`

2. **Tulis unit test** di folder `tests/Core.Tests/`  
   Buat file test baru (atau lengkapi file yang ada) untuk setiap layanan:
   - Gunakan **Stub** untuk menguji `PriceService`
   - Gunakan **Fake (in-memory repository)** untuk `RegistrationService`
   - Gunakan **Mock (Moq)** dan **Spy** untuk `OrderService`

3. **Jalankan test lokal**  
   ```bash
   dotnet restore
   dotnet test

4. Pastikan semua test lulus sebelum di-push ke GitHub.

5. **Push hasil ke GitHub**
   Setelah push, **GitHub Actions** akan otomatis:

   * Menjalankan test
   * Menghitung **line & branch coverage**
   * Memperbarui badge coverage di README
   * Gagal jika coverage < 80% (line) atau < 70% (branch)

5. **Tujuan akhir**

   * Semua test lulus ✅
   * Coverage badge berwarna **hijau** 🟢
   * Tidak ada modifikasi di folder `src/`

---

> **Penting:** Anda **hanya** menulis/menambah kode di folder `tests/`. Folder `src/` adalah konteks sistem yang diuji dan **tidak boleh diubah**.

---

