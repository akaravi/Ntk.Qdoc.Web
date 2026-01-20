# Ntk.Qdoc.Web

یک اپلیکیشن Blazor Server برای پیام‌رسانی سریع با ورود از طریق QR و اسکن بارکد. کاربران با یک کد تصادفی وارد می‌شوند، کد را به شکل QR دریافت می‌کنند و می‌توانند با اسکن یا وارد کردن کد دیگران، پیام خصوصی یا گروهی ردوبدل کنند.

## Tech Stack
- .NET 9 / Blazor Server
- C# 12, ASP.NET Core Hosting
- Channels برای صف پیام‌ها (Publisher/Consumer)
- QRCoder برای تولید QR، BlazorBarcodeScanner.ZXing.JS برای اسکن

## Features
- تولید QR اختصاصی برای هر نشست کاربر و نمایش آن در صفحه اصلی.
- اسکن یا ورود دستی کد مخاطب برای شروع گفتگوی خصوصی.
- فرم پیام‌رسانی بلادرنگ با نمایش تاریخچه پیام‌ها.
- اتاق گفتگوی مشترک (`/room`) با فهرست آنلاین/آفلاین کاربران.
- API ناشناس `POST /api/chat` برای ارسال پیام از بیرون برنامه.
- مدیریت اتصال کاربر به کمک `ClientCircuitHandler` و نگهداری وضعیت در حافظه.

## Architecture Overview
- Blazor Server با SignalR؛ ورود و خروج کلاینت در `ClientCircuitHandler`.
- پیام‌ها در `MessagesPublisher`/`MessagesConsumer` روی یک کانال محدود (۱۰۰ آیتم) قرار می‌گیرند و `MessagesConsumerWorker` آنها را پردازش می‌کند.
- منطق دامنه در `ChatService`: ورود/خروج کاربر، انتشار رویدادهای `UserLoggedIn`, `UserLoggedOut`, `MessageReceived` و ارسال پیام به همه یا به گیرنده مشخص.
- وضعیت کاربران در `UserStateProvider` (ConcurrentDictionary) نگهداری می‌شود؛ `InMemoryConnectedClientService` شناسه اتصال را ذخیره می‌کند.
- لایه UI شامل کامپوننت‌های Razor:
  - `Index.razor`: جریان اصلی، نمایش QR، جابه‌جایی بین اسکن، ارسال/دریافت و اتاق گفتگو.
  - `ScanDevice.razor`: اسکن/ورود کد و یافتن کاربر.
  - `MessageForm.razor`: ارسال پیام خصوصی یا عمومی.
  - `Chat.razor`: نمایش پیام‌ها؛ `Users.razor`: لیست وضعیت کاربران.

## Project Layout
- `Ntk.Qdoc.Web.Blazor/Startup.cs` پیکربندی DI، CORS، Blazor و پس‌زمینه.
- `Ntk.Qdoc.Web.Blazor/Pages` صفحات و کامپوننت‌های Razor.
- `Ntk.Qdoc.Web.Blazor/Services` سرویس‌های پیام، پس‌زمینه و اتصال.
- `Ntk.Qdoc.Web.Blazor/Interfaces` قرارداد سرویس‌ها.
- `Ntk.Qdoc.Web.Blazor/Models` مدل‌های دامنه و DTOها.
- `Ntk.Qdoc.Web.Blazor/Helper` ابزارهای QR و تولید کد تصادفی.
- `wwwroot` استایل‌ها، آیکون‌ها و دارایی‌های استاتیک.

## Getting Started
### Prerequisites
- .NET 9 SDK

### Restore & Run
```bash
dotnet restore
dotnet run --project Ntk.Qdoc.Web.Blazor
```
سپس به آدرس `https://localhost:5001` (یا پورت اعلام‌شده در لاگ) بروید.

### Configuration
- `appsettings.json` و `appsettings.Development.json` برای تنظیمات محیط. پیش‌فرض‌ها اغلب کافی است؛ CORS به‌صورت باز (`*`) تنظیم شده، در محیط واقعی دامنۀ مجاز را محدود کنید.
- ظرفیت صف پیام در `Startup` روی ۱۰۰ تنظیم شده است؛ در بار بالا متناسب‌سازی کنید.

### API Quick Test
```bash
curl -X POST https://localhost:5001/api/chat \
  -H "Content-Type: application/json" \
  -d '{ "username": "device-a", "message": "hello from api" }'
```
پیام ارسال‌شده در UI برای گیرنده ظاهر می‌شود.

## Development Notes
- مدل ورود: هر نشست یک کد عددی تصادفی دارد؛ QR آن کد را به صورت URL تولید می‌کند.
- پیام‌های خصوصی: هنگام دریافت لینک/متن با پیشوند `http(s)` برای گیرنده، ناوبری به URL انجام می‌شود.
- برای جلوگیری از نشت رویدادها، در کامپوننت‌ها unsubscribe در `Dispose` رعایت شده است.
- اگر چند زبان نیاز است، منابع متنی UI فارسی هستند؛ ترجمه‌های جدید را در فایل‌های مربوطه اضافه کنید.

## Troubleshooting
- پیام نیامد: بررسی کنید صف کانال پر نشده باشد (ظرفیت ۱۰۰) و مرورگر به سرور متصل باشد.
- اسکن کار نمی‌کند: دسترسی دوربین مرورگر را فعال کنید یا ورودی دستی کد را تست کنید.
- خروج ناگهانی مخاطب: `Index.razor` در رویداد خروج پیام هشدار می‌دهد؛ به صفحه اصلی برگردید.

## License
در صورت داشتن سیاست لایسنس، این بخش را به‌روز کنید.