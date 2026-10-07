![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![C%23](https://img.shields.io/badge/C%23-14-purple)
![NUnit](https://img.shields.io/badge/NUnit-4.x-green)
![Moq](https://img.shields.io/badge/Moq-4.x-orange)

# Tutorial Test Doubles dengan NUnit dan Moq

Tutorial ini menggunakan tiga service yang sudah tersedia: konversi harga, pendaftaran pengguna, dan pemesanan barang. Mahasiswa membuat unit test dengan mengganti dependensi menggunakan **Dummy, Stub, Fake, Spy, dan Mock**.

Tutorial dibagi menjadi empat bagian:

1. Stub untuk konversi harga.
2. Fake untuk pendaftaran pengguna.
3. Dummy dan Spy untuk pemesanan yang gagal.
4. Mock dengan Moq untuk interaksi pembayaran dan email.

Pada setiap bagian:

1. baca perilaku service;
2. tentukan dependensi yang perlu diganti;
3. siapkan test double;
4. implementasikan test dengan Arrange–Act–Assert; dan
5. jalankan serta evaluasi hasil pengujian.

---

## Capaian Pembelajaran

Setelah menyelesaikan tutorial ini, mahasiswa mampu:

1. menjelaskan peran test double dalam mengisolasi system under test (SUT);
2. membedakan Dummy, Stub, Fake, Spy, dan Mock;
3. membuat test double manual dan menggunakan Moq;
4. memeriksa hasil dan perubahan state;
5. memverifikasi interaksi yang menjadi bagian perilaku service; dan
6. menjalankan unit test menggunakan Visual Studio dan `dotnet test`.

---

## Lingkungan Pengembangan

- **IDE:** Visual Studio 2026 atau editor yang mendukung C#.
- **SDK:** .NET 10.
- **Bahasa:** C# 14.
- **Test framework:** NUnit 4.x.
- **Mocking library:** Moq 4.x.
- **Test runner:** Microsoft.NET.Test.Sdk dan NUnit3TestAdapter.

Package yang diperlukan sudah tercantum dalam `tests/Core.Tests/Core.Tests.csproj`. Tidak perlu membuat project baru atau menambahkan package untuk mengikuti tutorial ini.

Jalankan perintah dari direktori utama repository:

```bash
dotnet restore TestDoubles.sln
dotnet build TestDoubles.sln
dotnet test TestDoubles.sln
```

---

## Struktur Solution

```text
st-test-doubles-csharp-template/
├── TestDoubles.sln
├── README.md
├── src/
│   └── Core/
│       ├── Core.csproj
│       ├── Interfaces/
│       │   ├── IExchangeRate.cs
│       │   ├── IUserRepository.cs
│       │   ├── IEmailService.cs
│       │   ├── IPaymentGateway.cs
│       │   └── ILogger.cs
│       ├── Models/
│       │   ├── User.cs
│       │   └── Order.cs
│       └── Services/
│           ├── PriceService.cs
│           ├── RegistrationService.cs
│           └── OrderService.cs
└── tests/
    └── Core.Tests/
        ├── Core.Tests.csproj
        ├── PriceServiceTests.cs
        ├── RegistrationServiceTests.cs
        └── OrderServiceTests.cs
```

Ketentuan umum:

- Kode produksi di `src/` tidak boleh diubah.
- Lengkapi tiga file test yang tersedia dengan namespace `Core.Tests`.
- Tambahkan `[TestFixture]` pada class test.
- Gunakan nama method wajib yang disebutkan dalam tutorial.
- Test double manual dapat ditulis sebagai nested class privat di class test masing-masing.
- Setiap test membuat SUT dan dependensi baru; test tidak bergantung pada urutan eksekusi.
- Jangan menggunakan API kurs, database, email, atau payment gateway sungguhan.
- Jangan mengganti SUT dengan mock; yang diganti adalah dependensinya.
- Gunakan `decimal` untuk nominal, misalnya `2.5m`.
- Jangan menambah harapan validasi yang tidak tersedia dalam kode produksi.

Class test pada template masih kosong. Build berhasil atau perintah test selesai tanpa menemukan test belum berarti tugas selesai.

---

## Memulai Pengerjaan

1. Accept assignment melalui tautan Classroom50.
2. Clone repository assignment Anda:

```bash
git clone <URL_REPOSITORY_ANDA>
```

3. Masuk ke direktori hasil clone.
4. Buka `TestDoubles.sln`.
5. Jalankan restore dan build.
6. Baca kode ketiga service sebelum menulis test.

---

## Mengenali Peran Test Double

| Jenis | Peran dalam tutorial |
|---|---|
| Dummy | Mengisi parameter constructor yang tidak digunakan pada skenario tertentu |
| Stub | Memberikan jawaban yang ditentukan, misalnya kurs atau hasil pembayaran |
| Fake | Implementasi sederhana yang bekerja, misalnya repository in-memory |
| Spy | Merekam panggilan agar dapat diperiksa setelah SUT berjalan |
| Mock | Memverifikasi harapan interaksi, misalnya email dikirim tepat satu kali |

Moq adalah alat, bukan nama peran. Object Moq yang hanya memberikan hasil melalui `Setup(...).Returns(...)` berperan sebagai stub. Ketika digunakan untuk memverifikasi interaksi melalui `Verify(...)`, object tersebut berperan sebagai mock.

---

# Bagian 1. Stub: Konversi Harga

## 1.1 Tujuan

Mengendalikan nilai kurs agar pengujian deterministik.

## 1.2 Perilaku Service

`PriceService` menerima `IExchangeRate` melalui constructor:

```csharp
decimal GetRate(string currency);
```

Method yang diuji:

```csharp
decimal ConvertToIDR(decimal usd);
```

Service meminta kurs dengan kode `"USD"`, lalu mengembalikan `usd * rate`. Kode saat ini tidak menolak kurs nol dan tidak melakukan pembulatan tambahan.

## 1.3 Membuat Stub

Buka `PriceServiceTests.cs`. Tambahkan import berikut dan buat fixture dalam namespace `Core.Tests`:

```csharp
using Core.Interfaces;
using Core.Services;
using NUnit.Framework;
```

Tambahkan nested class berikut:

```csharp
private sealed class FixedExchangeRateStub : IExchangeRate
{
    private readonly decimal _rate;

    public FixedExchangeRateStub(decimal rate) => _rate = rate;

    public decimal GetRate(string currency)
    {
        if (currency != "USD")
            throw new ArgumentException("Kode kurs yang diharapkan adalah USD.");

        return _rate;
    }
}
```

Pemeriksaan kode kurs membuat stub tidak menyembunyikan kesalahan jika service meminta mata uang yang berbeda.

## 1.4 Implementasi Test Pertama

```csharp
[Test]
public void ConvertToIDR_WithFixedRate_ReturnsExpectedAmount()
{
    // Arrange
    var rate = new FixedExchangeRateStub(15_000m);
    var service = new PriceService(rate);

    // Act
    decimal actual = service.ConvertToIDR(10m);

    // Assert
    Assert.That(actual, Is.EqualTo(150_000m));
}
```

## 1.5 Test Case

| Method wajib | Kurs | Input USD | Expected IDR |
|---|---:|---:|---:|
| `ConvertToIDR_WithFixedRate_ReturnsExpectedAmount` | 15000 | 10 | 150000 |
| `ConvertToIDR_WithZeroRate_ReturnsZero` | 0 | 10 | 0 |
| `ConvertToIDR_WithDecimalAmount_PreservesPrecision` | 15250 | 2.5 | 38125 |

## 1.6 Tugas

1. Tambahkan dua test lainnya dengan nama sesuai tabel.
2. Buat stub baru untuk setiap test.
3. Periksa hasil dengan `Assert.That`.
4. Pastikan kurs nol menghasilkan nol tanpa exception.

## 1.7 Menjalankan Test

```bash
dotnet test TestDoubles.sln --filter "FullyQualifiedName~PriceServiceTests"
```

Minimal tiga test wajib harus terdeteksi dan lulus.

---

# Bagian 2. Fake: Pendaftaran Pengguna

## 2.1 Tujuan

Menggunakan repository in-memory untuk memeriksa hasil pendaftaran dan data tersimpan.

## 2.2 Perilaku Service

Model pengguna:

```csharp
public record User(string Username);
```

Interface repository:

```csharp
public interface IUserRepository
{
    void Add(User user);
    User? Find(string username);
}
```

`RegistrationService.Register(User user)`:

- mengembalikan `false` jika username sudah ditemukan;
- jika belum ditemukan, menambahkan user dan mengembalikan `true`.

## 2.3 Menyiapkan Fake Repository

Buka `RegistrationServiceTests.cs`. Gunakan import `Core.Interfaces`, `Core.Models`, `Core.Services`, dan `NUnit.Framework`.

Tambahkan nested class berikut dan lengkapi kedua method:

```csharp
private sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public int Count => _users.Count;

    public void Add(User user)
    {
        // TODO: simpan user dalam koleksi.
    }

    public User? Find(string username)
    {
        // TODO: cari user berdasarkan Username dengan StringComparison.Ordinal.
        throw new NotImplementedException();
    }
}
```

Gunakan `FirstOrDefault` untuk mengembalikan user yang cocok atau `null`. Fake menyimpan state, bukan selalu memberikan jawaban yang sama.

## 2.4 Menulis Test Pendaftaran Baru

```csharp
[Test]
public void Register_WithNewUsername_SavesUserAndReturnsTrue()
{
    // Arrange
    var repository = new InMemoryUserRepository();
    var service = new RegistrationService(repository);
    var user = new User("alya");

    // Act
    bool result = service.Register(user);

    // Assert
    // TODO: periksa result, user hasil Find("alya"), dan Count.
}
```

## 2.5 Test Case

| Method wajib | Persiapan dan tindakan | Hasil yang diperiksa |
|---|---|---|
| `Register_WithNewUsername_SavesUserAndReturnsTrue` | Daftarkan alya pada repository kosong | `true`; user tersimpan; jumlah 1 |
| `Register_WithDuplicateUsername_ReturnsFalseAndPreservesData` | Daftarkan alya; daftarkan alya lagi | Pertama `true`, kedua `false`; jumlah tetap 1; data awal tetap |
| `Register_WithTwoDifferentUsers_SavesBothUsers` | Daftarkan alya dan bima | Keduanya `true`; kedua user dapat ditemukan; jumlah 2 |

## 2.6 Tugas

1. Lengkapi fake repository.
2. Implementasikan tiga test wajib.
3. Untuk menyiapkan duplikasi, daftarkan user melalui service dan pastikan pendaftaran awal berhasil.
4. Periksa nilai kembalian serta data tersimpan; assertion jumlah saja belum cukup membuktikan isi benar.

## 2.7 Menjalankan Test

```bash
dotnet test TestDoubles.sln --filter "FullyQualifiedName~RegistrationServiceTests"
```

Minimal tiga test wajib harus terdeteksi dan lulus.

---

# Bagian 3. Dummy dan Spy: Pembayaran Gagal

## 3.1 Tujuan

Memahami dependensi yang tidak digunakan pada cabang tertentu dan merekam efek samping logging.

## 3.2 Perilaku OrderService

Model pesanan:

```csharp
public record Order(string CustomerEmail, decimal Total);
```

Dependensi:

```csharp
bool Charge(decimal amount);                  // IPaymentGateway
void SendEmail(string address, string message); // IEmailService
void Log(string message);                     // ILogger
```

Constructor dan method SUT:

```csharp
new OrderService(email, gateway, logger);
bool result = service.PlaceOrder(order);
```

| Hasil Charge | Hasil PlaceOrder | Email | Log |
|---|---|---|---|
| `true` | `true` | Alamat pelanggan, pesan `"Order confirmed!"`, sekali | Tidak ada |
| `false` | `false` | Tidak ada | Pesan `"Payment failed"`, sekali |

Pada kedua cabang, `Charge` menerima `order.Total` tepat satu kali.

## 3.3 Membuat Dummy Email

Buka `OrderServiceTests.cs`. Gunakan import `Core.Interfaces`, `Core.Models`, `Core.Services`, `Moq`, dan `NUnit.Framework`.

```csharp
private sealed class DummyEmailService : IEmailService
{
    public void SendEmail(string address, string message)
        => throw new InvalidOperationException(
            "Dummy email tidak boleh digunakan pada skenario ini.");
}
```

Dummy mengisi parameter constructor pada skenario pembayaran gagal, ketika email seharusnya tidak digunakan. Method dibuat melempar exception agar pemakaian yang tidak diharapkan langsung terlihat.

## 3.4 Membuat Stub Pembayaran dan Spy Logger

```csharp
private sealed class PaymentGatewayStub : IPaymentGateway
{
    private readonly bool _result;

    public PaymentGatewayStub(bool result) => _result = result;

    public bool Charge(decimal amount) => _result;
}

private sealed class LoggerSpy : ILogger
{
    public List<string> Messages { get; } = new();

    public void Log(string message)
    {
        // TODO: rekam pesan ke Messages.
    }
}
```

## 3.5 Implementasi Test

```csharp
[Test]
public void PlaceOrder_WhenPaymentFails_WithDummyEmail_ReturnsFalseAndLogsFailure()
{
    // Arrange
    var email = new DummyEmailService();
    var gateway = new PaymentGatewayStub(false);
    var logger = new LoggerSpy();
    var service = new OrderService(email, gateway, logger);
    var order = new Order("alya@example.test", 100_000m);

    // Act
    bool result = service.PlaceOrder(order);

    // Assert
    // TODO: result false; tepat satu pesan; pesan sama dengan "Payment failed".
}
```

Test ini memiliki tiga pemeriksaan: hasil operasi, rekaman logging, dan dummy email tidak digunakan.

## 3.6 Tugas

1. Lengkapi `LoggerSpy.Log`.
2. Lengkapi assertion pada test di atas.
3. Jangan hanya memeriksa bahwa daftar pesan tidak kosong.
4. Simpan test dalam `OrderServiceTests` agar dapat dijalankan bersama Bagian 4.

## 3.7 Menjalankan Test

```bash
dotnet test TestDoubles.sln --filter "FullyQualifiedName~PlaceOrder_WhenPaymentFails_WithDummyEmail_ReturnsFalseAndLogsFailure"
```

Satu test wajib harus terdeteksi dan lulus.

---

# Bagian 4. Mock: Memverifikasi Interaksi dengan Moq

## 4.1 Tujuan

Memverifikasi nominal pembayaran, tujuan dan isi email, jumlah panggilan, serta operasi yang tidak boleh terjadi.

## 4.2 Menyiapkan Dependensi

Untuk test pembayaran berhasil:

```csharp
var email = new Mock<IEmailService>();
var gateway = new Mock<IPaymentGateway>();
var logger = new LoggerSpy();

gateway
    .Setup(g => g.Charge(100_000m))
    .Returns(true);

var service = new OrderService(email.Object, gateway.Object, logger);
var order = new Order("alya@example.test", 100_000m);
```

Pada contoh ini, konfigurasi hasil pembayaran berperan sebagai stub. Verifikasi panggilan menggunakan Moq berperan sebagai mock.

## 4.3 Memverifikasi Pembayaran dan Email

Setelah `PlaceOrder(order)` dipanggil:

```csharp
gateway.Verify(g => g.Charge(order.Total), Times.Once);
gateway.VerifyNoOtherCalls();

email.Verify(
    e => e.SendEmail(order.CustomerEmail, "Order confirmed!"),
    Times.Once);
email.VerifyNoOtherCalls();
```

`VerifyNoOtherCalls` digunakan setelah memverifikasi panggilan yang diharapkan agar panggilan tambahan dengan argumen berbeda juga terdeteksi.

Lengkapi assertion nilai kembalian `true` dan `logger.Messages` kosong.

## 4.4 Test Pembayaran Gagal

Siapkan mock gateway yang mengembalikan `false`, mock email, dan `LoggerSpy`.

Periksa:

1. `PlaceOrder` mengembalikan `false`.
2. `Charge(order.Total)` dipanggil tepat sekali, tanpa panggilan pembayaran tambahan.
3. Tidak ada email dengan argumen apa pun:

```csharp
email.Verify(
    e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>()),
    Times.Never);
```

4. Spy merekam tepat satu pesan `"Payment failed"`.

## 4.5 Test Case Wajib

Gunakan nominal dan email berikut agar skenario tidak hanya memeriksa nilai contoh pertama.

| Method wajib | Input | Pemeriksaan utama |
|---|---|---|
| `PlaceOrder_WhenPaymentSucceeds_ChargesAndSendsConfirmationOnce` | alya@example.test; 100000 | Hasil true; Charge dan email tepat sekali; log kosong |
| `PlaceOrder_WhenPaymentFails_ChargesOnceAndDoesNotSendEmail` | bima@example.test; 75000 | Hasil false; Charge tepat sekali; tidak ada email; log tepat |
| `PlaceOrder_WithDifferentOrder_UsesOrderTotalAndCustomerEmail` | citra@example.test; 125000.5 | Hasil true; nominal, alamat, dan pesan tepat; tidak ada panggilan tambahan; log kosong |

## 4.6 Tugas

1. Implementasikan ketiga test wajib dengan nama sesuai tabel.
2. Gunakan `Verify` untuk memeriksa argumen dan jumlah panggilan.
3. Gunakan `LoggerSpy` untuk memeriksa logging.
4. Buat object mock baru pada setiap test.
5. Jangan hanya menggunakan `It.IsAny<decimal>()` atau `It.IsAny<string>()` untuk verifikasi panggilan yang harus memiliki argumen tepat.

## 4.7 Pengayaan Opsional: Urutan Interaksi

Tambahkan test bernama `PlaceOrder_WhenPaymentSucceeds_ChargesBeforeSendingEmail`.

Gunakan `Callback` pada mock pembayaran dan email untuk menambahkan penanda `"charge"` dan `"email"` ke list lokal. Pastikan pembayaran mengembalikan `true`, lalu periksa urutan list tepat `["charge", "email"]`.

Test pengayaan ini tidak termasuk jumlah test wajib.

## 4.8 Menjalankan Test

```bash
dotnet test TestDoubles.sln --filter "FullyQualifiedName~OrderServiceTests"
```

Minimal empat test wajib harus terdeteksi dan lulus: satu dari Bagian 3 dan tiga dari Bagian 4.

---

# Menjalankan Seluruh Test

```bash
dotnet restore TestDoubles.sln
dotnet build TestDoubles.sln --configuration Release --no-restore
dotnet test TestDoubles.sln --configuration Release --no-build --no-restore
```

Checklist pengerjaan:

- Minimal **10 test wajib** terdeteksi: 3 PriceService, 3 RegistrationService, dan 4 OrderService.
- Setiap method wajib memiliki atribut `[Test]`.
- Seluruh test lulus dan tidak diabaikan.
- Stub, Fake, Dummy, Spy, dan Mock digunakan sesuai bagian tutorial.
- Seluruh kode produksi di `src/` tidak berubah.

Di Visual Studio, gunakan **Test > Test Explorer > Run All Tests**.

---
