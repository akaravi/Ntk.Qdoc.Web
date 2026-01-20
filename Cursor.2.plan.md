# Plan برای تغییرات پروژه Ntk.Qdoc.Web

## Part 0: Phase 1 پیاده‌سازی شده ✅

## Part 0.5: Phase 2 پیاده‌سازی شده ✅
Commands:
- ایجاد ChatThreadModel.cs برای مدیریت thread های چت
- ایجاد IChatThreadService.cs و ChatThreadService.cs برای مدیریت thread ها
- ایجاد IMessageRepository.cs و InMemoryMessageRepository.cs برای ذخیره پیام‌ها
- تغییر MessageModel.cs برای اضافه کردن ChatId و پشتیبانی از چت گروهی
- تغییر ChatService.cs و IChatService.cs برای اضافه کردن متدهای چت گروهی
- تغییر MessagesConsumerWorker.cs برای ذخیره خودکار پیام‌ها در repository
- ثبت Services جدید در Startup.cs
- تغییر Chat.razor برای پشتیبانی از ChatId و بارگذاری تاریخچه
- تغییر MessageForm.razor برای پشتیبانی از ChatId
- تغییر Users.razor برای امکان انتخاب چند کاربر (Multi-select)
- ایجاد MultiChatView.razor component برای نمایش و مدیریت چت‌های چندگانه
- اضافه کردن استایل‌های MultiChatView به site.css
- تغییر Index.razor برای اضافه کردن ViewMultiChatComponent

Result 0.5:
- قابلیت چت چندنفره (Group Chat) کامل پیاده‌سازی شد
- کاربران می‌توانند چت گروهی ایجاد کنند
- تاریخچه پیام‌ها ذخیره و قابل دسترسی است
- UI برای مدیریت چندین چت به صورت همزمان آماده است
- تمام Services و Models برای چت گروهی آماده هستند

---

## Part 0: Phase 1 پیاده‌سازی شده ✅
Commands:
- ایجاد صفحه About.razor با نمایش نسخه و اطلاعات تکنولوژی
- ایجاد صفحه Contact.razor با فرم تماس کامل و validation
- ایجاد صفحه Help.razor با راهنمای کامل استفاده
- ایجاد صفحه ApiDocs.razor با مستندات کامل API و مثال‌های کد (curl, JavaScript, C#, Python)
- بهبود MainLayout.razor با Navigation Menu کامل و responsive
- اضافه کردن تمام استایل‌های مورد نیاز به site.css
- ایجاد ContactViewModel.cs برای validation فرم تماس

Result 0:
- صفحات About, Contact, Help, ApiDocs ایجاد و آماده استفاده هستند
- Navigation Menu کامل با لینک‌های تمام صفحات
- استایل‌های responsive برای تمام صفحات جدید
- فرم تماس با validation کامل
- مستندات API با مثال‌های کاربردی در چند زبان

---

## هدف
این plan شامل جزئیات پیاده‌سازی 5 ویژگی اصلی است:
1. چت همزمان با چند نفر
2. صفحه راهنما و کاربری
3. صفحه درباره ما
4. صفحه تماس با ما
5. صفحه راهنمای API برای برنامه‌نویسان

---

## Part 1: چت همزمان با چند نفر

### تحلیل وضعیت فعلی:
- **MessageModel**: فعلاً فقط یک `ReceiverUsername` دارد که برای پیام خصوصی استفاده می‌شود
- **Chat.razor**: تمام پیام‌ها را در یک لیست نمایش می‌دهد بدون جداسازی بر اساس چت
- **Index.razor**: فقط یک `ReceiverUsername` را نگهداری می‌کند
- **ChatService**: فقط دو متد دارد: `PostMessageAsync(user, message)` برای عمومی و `PostMessageAsync(user, receiver, message)` برای خصوصی

### تغییرات مورد نیاز:

#### 1.1 ایجاد Model برای چت‌های چندنفره
**فایل:** `Ntk.Qdoc.Web.Blazor/Models/ChatThreadModel.cs` (جدید)
- Properties: `ChatId` (string), `Participants` (List<string>), `LastMessageTime` (DateTime), `UnreadCount` (int)
- Methods: `AddParticipant`, `RemoveParticipant`

#### 1.2 تغییر MessageModel
**فایل:** `Ntk.Qdoc.Web.Blazor/Models/MessageModel.cs`
- اضافه کردن `ChatId` property (اختیاری - برای چت‌های گروهی)
- اگر `ChatId` موجود باشد، `ReceiverUsername` نادیده گرفته می‌شود
- حفظ سازگاری با کد فعلی

#### 1.3 تغییر ChatService
**فایل:** `Ntk.Qdoc.Web.Blazor/Services/ChatService.cs`
- اضافه کردن `CreateChatThreadAsync(List<string> participants)` - ایجاد چت گروهی جدید
- اضافه کردن `GetChatThreadsForUser(string username)` - دریافت لیست چت‌های یک کاربر
- اضافه کردن `PostMessageToChatAsync(string chatId, UserModel user, string message)` - ارسال پیام به چت گروهی
- اضافه کردن `GetMessagesForChat(string chatId)` - دریافت پیام‌های یک چت
- تغییر متد `PostMessageAsync` موجود برای پشتیبانی از `ChatId`

#### 1.4 ایجاد ChatThreadService
**فایل:** `Ntk.Qdoc.Web.Blazor/Services/ChatThreadService.cs` (جدید)
- مدیریت چت‌های گروهی
- نگهداری `Dictionary<string, ChatThreadModel>` برای چت‌ها
- متدهای: `Create`, `GetByChatId`, `GetByUser`, `AddParticipant`, `RemoveParticipant`

#### 1.5 تغییر Chat.razor Component
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Components/Chat.razor`
- اضافه کردن Parameter `ChatId` (اختیاری)
- اگر `ChatId` موجود باشد، فقط پیام‌های آن چت را نمایش دهد
- فیلتر کردن پیام‌ها بر اساس `ChatId` یا `ReceiverUsername` + `Username`
- نگهداری لیست جداگانه پیام‌ها برای هر چت

#### 1.6 ایجاد MultiChatView Component
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Components/MultiChatView.razor` (جدید)
- نمایش لیست چت‌های فعال در سمت چپ (مشابه WhatsApp)
- امکان انتخاب چت برای نمایش
- نمایش چت انتخاب‌شده در سمت راست
- قابلیت ایجاد چت جدید با انتخاب چند کاربر

#### 1.7 تغییر Index.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Index.razor`
- اضافه کردن حالت نمایش `ViewMultiChatComponent`
- پیاده‌سازی `MultiChatView` component
- حفظ سازگاری با حالت‌های قبلی

#### 1.8 تغییر MessageForm.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Components/MessageForm.razor`
- اضافه کردن Parameter `ChatId` (اختیاری)
- اگر `ChatId` موجود باشد، از `PostMessageToChatAsync` استفاده کند
- حفظ سازگاری با کد فعلی

#### 1.9 تغییر Users.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Components/Users.razor`
- اضافه کردن امکان انتخاب چند کاربر (checkbox)
- دکمه "شروع چت گروهی" برای ایجاد چت با کاربران انتخاب‌شده

#### 1.10 تغییر Startup.cs
**فایل:** `Ntk.Qdoc.Web.Blazor/Startup.cs`
- اضافه کردن `ChatThreadService` به DI container به عنوان Singleton

---

## Part 2: صفحه راهنما و کاربری (Help/Guide)

### 2.1 ایجاد HelpPage.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Help.razor` (جدید)
**Route:** `@page "/help"`
- عنوان: "راهنمای استفاده"
- بخش‌ها:
  - **شروع کار**: نحوه اسکن QR و ورود
  - **ارسال پیام**: نحوه ارسال پیام خصوصی و عمومی
  - **چت گروهی**: نحوه ایجاد و مدیریت چت‌های گروهی
  - **استفاده از API**: لینک به صفحه API Documentation
  - **سوالات متداول (FAQ)**: پاسخ به سوالات رایج
  - **عیب‌یابی**: حل مشکلات رایج

### 2.2 ایجاد HelpPage.razor.cs (Code-behind)
- متدهای helper برای نمایش بخش‌های مختلف
- مدیریت state برای accordion/expandable sections

### 2.3 استایل‌های Help Page
**فایل:** `Ntk.Qdoc.Web.Blazor/wwwroot/css/site.css`
- `.help-section`: استایل برای هر بخش
- `.help-step`: استایل برای مراحل راهنما
- `.help-faq`: استایل برای سوالات متداول
- `.help-troubleshooting`: استایل برای عیب‌یابی

---

## Part 3: صفحه درباره ما (About)

### 3.1 ایجاد AboutPage.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/About.razor` (جدید)
**Route:** `@page "/about"`
- عنوان: "درباره ما"
- محتوا:
  - معرفی پروژه
  - تکنولوژی‌های استفاده‌شده (.NET 9, Blazor Server, SignalR)
  - ویژگی‌های کلیدی
  - نسخه نرم‌افزار (خواندن از csproj)
  - لینک به GitHub repository (اگر موجود باشد)
  - لایسنس

### 3.2 خواندن Version از csproj
- استفاده از `System.Reflection` برای خواندن AssemblyVersion
- نمایش در صفحه About

---

## Part 4: صفحه تماس با ما (Contact)

### 4.1 ایجاد ContactPage.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Contact.razor` (جدید)
**Route:** `@page "/contact"`
- عنوان: "تماس با ما"
- فرم تماس:
  - نام (InputText)
  - ایمیل (InputText, validation)
  - موضوع (InputSelect)
  - پیام (InputTextArea)
  - دکمه ارسال

### 4.2 ایجاد ContactViewModel
**فایل:** `Ntk.Qdoc.Web.Blazor/ViewModels/ContactViewModel.cs` (جدید)
- Properties: `Name`, `Email`, `Subject`, `Message`
- Data Annotations برای validation

### 4.3 ایجاد ContactService (اختیاری)
**فایل:** `Ntk.Qdoc.Web.Blazor/Services/ContactService.cs` (جدید)
- متد `SendContactMessageAsync(ContactViewModel model)`
- می‌تواند پیام را در log ذخیره کند یا به یک email service ارسال کند

### 4.4 ایجاد ContactController (اختیاری)
**فایل:** `Ntk.Qdoc.Web.Blazor/Controller/ContactController.cs` (جدید)
- Endpoint: `POST /api/contact`
- دریافت ContactViewModel و پردازش آن

---

## Part 5: صفحه راهنمای API (API Documentation)

### 5.1 ایجاد ApiDocsPage.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/ApiDocs.razor` (جدید)
**Route:** `@page "/api-docs"`
- عنوان: "راهنمای API"
- بخش‌ها:

#### 5.1.1 معرفی API
- Base URL: `https://your-domain.com/api/chat`
- Authentication: فعلاً Anonymous (AllowAnonymous)
- Format: JSON

#### 5.1.2 Endpoint: POST /api/chat
- **توضیحات**: ارسال پیام از طریق API
- **Request Body**:
  ```json
  {
    "username": "string",
    "message": "string",
    "receiverUsername": "string (optional)"
  }
  ```
- **Response**:
  ```json
  {
    "success": true,
    "messageId": "guid"
  }
  ```
- **Status Codes**: 200 OK, 400 Bad Request, 500 Internal Server Error
- **مثال با curl**:
  ```bash
  curl -X POST https://your-domain.com/api/chat \
    -H "Content-Type: application/json" \
    -d '{"username":"device-a","message":"Hello from API"}'
  ```
- **مثال با JavaScript (fetch)**:
  ```javascript
  fetch('https://your-domain.com/api/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      username: 'device-a',
      message: 'Hello from API',
      receiverUsername: 'device-b' // optional
    })
  })
  ```
- **مثال با C# (HttpClient)**:
  ```csharp
  var client = new HttpClient();
  var content = new StringContent(
    JsonSerializer.Serialize(new {
      username = "device-a",
      message = "Hello from API"
    }),
    Encoding.UTF8,
    "application/json"
  );
  var response = await client.PostAsync("https://your-domain.com/api/chat", content);
  ```
- **مثال با Python (requests)**:
  ```python
  import requests
  response = requests.post(
    'https://your-domain.com/api/chat',
    json={
      'username': 'device-a',
      'message': 'Hello from API'
    }
  )
  ```

#### 5.1.3 Model Schema
- نمایش SendMessageDtoModel structure
- Validation rules

#### 5.1.4 Rate Limiting & Best Practices
- توصیه‌ها برای استفاده بهینه
- محدودیت‌ها (اگر وجود دارد)

### 5.2 بهبود ChatController
**فایل:** `Ntk.Qdoc.Web.Blazor/Controller/ChatController.cs`
- اضافه کردن XML Comments برای Swagger (اگر استفاده شود)
- اضافه کردن Response Types
- بهبود error handling

### 5.3 Code Examples Component
**فایل:** `Ntk.Qdoc.Web.Blazor/Pages/Components/CodeExample.razor` (جدید)
- Component برای نمایش مثال‌های کد
- Syntax highlighting (می‌توان از highlight.js استفاده کرد)
- Tab برای انتخاب زبان (curl, JavaScript, C#, Python)

---

## Part 6: Navigation & Layout

### 6.1 تغییر MainLayout.razor
**فایل:** `Ntk.Qdoc.Web.Blazor/Shared/MainLayout.razor`
- اضافه کردن Navigation Menu:
  - Home (صفحه اصلی)
  - راهنما (Help)
  - درباره ما (About)
  - تماس با ما (Contact)
  - API Documentation
- استفاده از NavLink components
- Responsive design برای موبایل

### 6.2 ایجاد NavigationMenu Component
**فایل:** `Ntk.Qdoc.Web.Blazor/Shared/NavigationMenu.razor` (جدید - یا بهبود NavMenu.razor موجود)
- منوی کامل با لینک‌های بالا
- استایل‌های modern
- Active state highlighting

---

## Part 7: بهبودهای UI/UX

### 7.1 بهبود استایل‌های صفحات جدید
**فایل:** `Ntk.Qdoc.Web.Blazor/wwwroot/css/site.css`
- استایل‌های مشترک برای صفحات Help, About, Contact, ApiDocs
- حفظ consistency با طراحی فعلی (dark theme)

### 7.2 Responsive Design
- اطمینان از responsive بودن تمام صفحات جدید
- Mobile-first approach

---

## Part 8: Testing & Validation

### 8.1 تست چت چندنفره
- تست ایجاد چت گروهی
- تست ارسال پیام به چت گروهی
- تست نمایش پیام‌ها در چت‌های مختلف
- تست فیلتر کردن پیام‌ها

### 8.2 تست صفحات جدید
- تست navigation بین صفحات
- تست فرم تماس
- تست نمایش محتوا

### 8.3 تست API
- تست API endpoint با ابزارهای مختلف
- تست error handling

---

## ترتیب پیاده‌سازی پیشنهادی:

### Phase 1: زیرساخت و Navigation
1. ایجاد صفحات About, Contact, Help, ApiDocs (ساده - بدون محتوای کامل)
2. ایجاد Navigation Menu
3. تست routing و navigation

### Phase 2: محتوای صفحات
1. کامل کردن صفحه About
2. کامل کردن صفحه Help
3. کامل کردن فرم Contact
4. کامل کردن صفحه API Docs

### Phase 3: چت چندنفره
1. ایجاد ChatThreadService
2. تغییر MessageModel
3. تغییر ChatService
4. ایجاد MultiChatView component
5. تغییر Chat.razor
6. تغییر Index.razor
7. تست کامل

---

## فایل‌های جدید که باید ایجاد شوند:

1. `Ntk.Qdoc.Web.Blazor/Models/ChatThreadModel.cs`
2. `Ntk.Qdoc.Web.Blazor/Services/ChatThreadService.cs`
3. `Ntk.Qdoc.Web.Blazor/Pages/Help.razor`
4. `Ntk.Qdoc.Web.Blazor/Pages/About.razor`
5. `Ntk.Qdoc.Web.Blazor/Pages/Contact.razor`
6. `Ntk.Qdoc.Web.Blazor/Pages/ApiDocs.razor`
7. `Ntk.Qdoc.Web.Blazor/Pages/Components/MultiChatView.razor`
8. `Ntk.Qdoc.Web.Blazor/Pages/Components/CodeExample.razor`
9. `Ntk.Qdoc.Web.Blazor/ViewModels/ContactViewModel.cs`
10. `Ntk.Qdoc.Web.Blazor/Services/ContactService.cs` (اختیاری)
11. `Ntk.Qdoc.Web.Blazor/Controller/ContactController.cs` (اختیاری)

---

## فایل‌های موجود که باید تغییر کنند:

1. `Ntk.Qdoc.Web.Blazor/Models/MessageModel.cs` - اضافه کردن ChatId
2. `Ntk.Qdoc.Web.Blazor/Services/ChatService.cs` - متدهای جدید برای چت گروهی
3. `Ntk.Qdoc.Web.Blazor/Pages/Components/Chat.razor` - پشتیبانی از ChatId
4. `Ntk.Qdoc.Web.Blazor/Pages/Components/MessageForm.razor` - پشتیبانی از ChatId
5. `Ntk.Qdoc.Web.Blazor/Pages/Components/Users.razor` - امکان انتخاب چند کاربر
6. `Ntk.Qdoc.Web.Blazor/Pages/Index.razor` - اضافه کردن MultiChatView
7. `Ntk.Qdoc.Web.Blazor/Shared/MainLayout.razor` - اضافه کردن Navigation
8. `Ntk.Qdoc.Web.Blazor/Startup.cs` - ثبت ChatThreadService
9. `Ntk.Qdoc.Web.Blazor/Controller/ChatController.cs` - بهبود documentation
10. `Ntk.Qdoc.Web.Blazor/wwwroot/css/site.css` - استایل‌های جدید

---

## نکات مهم:

1. **سازگاری معکوس**: تمام تغییرات باید با کد فعلی سازگار باشند
2. **Testing**: هر بخش باید بعد از پیاده‌سازی تست شود
3. **Performance**: در نظر گرفتن performance برای چت‌های چندنفره
4. **Security**: بررسی security برای API endpoints
5. **Documentation**: مستندسازی کدهای جدید
6. **Multilingual**: در نظر گرفتن چندزبانه بودن (فارسی به عنوان زبان پایه)

---

## Result

پس از پیاده‌سازی کامل این plan:
- کاربران می‌توانند همزمان با چند نفر چت کنند
- صفحات راهنما، درباره ما، تماس با ما و API Documentation در دسترس هستند
- Navigation کامل و user-friendly است
- API مستندسازی شده و مثال‌های کاربردی دارد

---

## Part 9: جزئیات پیاده‌سازی چت چندنفره (جزئیات بیشتر)

### 9.1 تحلیل دقیق Message Routing فعلی

**وضعیت فعلی در Index.razor (خط 128-150):**
```csharp
private async void OnMessage(object sender, MessageModel message)
{
    if (message.Username == User.Username || message.ReceiverUsername == User.Username)
    {
        // فیلتر پیام‌ها بر اساس Username یا ReceiverUsername
    }
}
```

**مشکل:** این روش فقط برای چت 1-to-1 کار می‌کند و نمی‌تواند چت‌های گروهی را مدیریت کند.

**راه حل:** باید پیام‌ها را بر اساس `ChatId` فیلتر کنیم:
- اگر `ChatId` موجود باشد، فقط اعضای آن چت پیام را ببینند
- اگر `ChatId` null باشد، از منطق فعلی (1-to-1) استفاده شود

### 9.2 ساختار ChatThreadModel

```csharp
public class ChatThreadModel
{
    public string ChatId { get; set; } = Guid.NewGuid().ToString();
    public List<string> Participants { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageTime { get; set; }
    public string LastMessagePreview { get; set; }
    public Dictionary<string, int> UnreadCounts { get; set; } = new Dictionary<string, int>();
    public string ChatName { get; set; } // برای چت‌های گروهی
    
    public void AddParticipant(string username)
    {
        if (!Participants.Contains(username))
            Participants.Add(username);
    }
    
    public void RemoveParticipant(string username)
    {
        Participants.Remove(username);
    }
    
    public bool IsParticipant(string username)
    {
        return Participants.Contains(username);
    }
}
```

### 9.3 تغییر MessageModel (جزئیات)

```csharp
public class MessageModel
{
    public MessageModel(string username, string text, DateTime when)
    {
        Username = username;
        Text = text;
        When = when;
        ChatId = null; // پیام عمومی
        ReceiverUsername = null;
    }
    
    public MessageModel(string username, string receiverUsername, string text, DateTime when)
    {
        Username = username;
        ReceiverUsername = receiverUsername;
        Text = text;
        When = when;
        ChatId = null; // پیام خصوصی 1-to-1
    }
    
    public MessageModel(string username, string chatId, string text, DateTime when, bool isGroupChat = true)
    {
        Username = username;
        ChatId = chatId;
        Text = text;
        When = when;
        ReceiverUsername = null; // در چت گروهی receiver نداریم
    }
    
    public string Username { get; }
    public string ReceiverUsername { get; } // فقط برای چت 1-to-1
    public string ChatId { get; } // برای چت‌های گروهی
    public string Text { get; }
    public DateTime When { get; }
    public bool IsGroupChat => !string.IsNullOrEmpty(ChatId);
}
```

### 9.4 پیاده‌سازی ChatThreadService (جزئیات کامل)

**Interface:** `IChatThreadService.cs`
```csharp
public interface IChatThreadService
{
    ChatThreadModel CreateChatThread(List<string> participants, string createdBy, string chatName = null);
    ChatThreadModel GetChatThread(string chatId);
    IEnumerable<ChatThreadModel> GetChatThreadsForUser(string username);
    void AddParticipant(string chatId, string username);
    void RemoveParticipant(string chatId, string username);
    void UpdateLastMessage(string chatId, DateTime when, string preview);
    void IncrementUnreadCount(string chatId, string username);
    void ResetUnreadCount(string chatId, string username);
}
```

**Implementation:** `ChatThreadService.cs`
- استفاده از `ConcurrentDictionary<string, ChatThreadModel>` برای thread-safe operations
- تولید ChatId با GUID
- مدیریت UnreadCounts برای هر کاربر

### 9.5 تغییر ChatService برای چت گروهی

**متدهای جدید:**
```csharp
// ایجاد چت گروهی
public async Task<ChatThreadModel> CreateGroupChatAsync(List<string> participants, string createdBy, string chatName = null)
{
    var chatThread = _chatThreadService.CreateChatThread(participants, createdBy, chatName);
    // می‌توان event برای اطلاع سایر کاربران ارسال کرد
    return chatThread;
}

// ارسال پیام به چت گروهی
public async Task PostMessageToChatAsync(string chatId, UserModel user, string message)
{
    var chatThread = _chatThreadService.GetChatThread(chatId);
    if (chatThread == null || !chatThread.IsParticipant(user.Username))
        throw new UnauthorizedAccessException("User is not a participant");
    
    var msg = new MessageModel(user.Username, chatId, message, DateTime.UtcNow, true);
    await _publisher.PublishAsync(msg);
    
    // به‌روزرسانی LastMessageTime و Preview
    _chatThreadService.UpdateLastMessage(chatId, msg.When, message);
    
    // افزایش UnreadCount برای سایر اعضا
    foreach (var participant in chatThread.Participants)
    {
        if (participant != user.Username)
            _chatThreadService.IncrementUnreadCount(chatId, participant);
    }
}

// دریافت پیام‌های یک چت
public IEnumerable<MessageModel> GetMessagesForChat(string chatId, string username)
{
    var chatThread = _chatThreadService.GetChatThread(chatId);
    if (chatThread == null || !chatThread.IsParticipant(username))
        return Enumerable.Empty<MessageModel>();
    
    // در حال حاضر همه پیام‌ها در Consumer ذخیره نمی‌شوند
    // باید یک MessageRepository اضافه شود یا از SignalR Hub استفاده شود
    // این یک محدودیت فعلی است که باید حل شود
    return _allMessages.Where(m => m.ChatId == chatId);
}
```

### 9.6 مشکل ذخیره‌سازی پیام‌ها

**مشکل فعلی:** 
- `MessagesConsumer` پیام‌ها را فقط broadcast می‌کند و ذخیره نمی‌کند
- برای چت‌های چندنفره نیاز به تاریخچه پیام‌ها داریم

**راه حل‌ها:**

**گزینه 1: In-Memory Storage (ساده - برای MVP)**
```csharp
public class MessageRepository : IMessageRepository
{
    private readonly ConcurrentDictionary<string, List<MessageModel>> _chatMessages;
    
    public void AddMessage(MessageModel message)
    {
        var key = message.ChatId ?? $"private_{GetPrivateChatKey(message.Username, message.ReceiverUsername)}";
        _chatMessages.AddOrUpdate(key, 
            new List<MessageModel> { message },
            (k, list) => { list.Add(message); return list; });
    }
    
    public IEnumerable<MessageModel> GetMessages(string chatId, int limit = 100)
    {
        if (_chatMessages.TryGetValue(chatId, out var messages))
            return messages.TakeLast(limit);
        return Enumerable.Empty<MessageModel>();
    }
}
```

**گزینه 2: استفاده از SignalR Hub برای Real-time (پیشنهادی)**
- SignalR Hub می‌تواند پیام‌ها را به کاربران مشخص ارسال کند
- بهتر است از Hub استفاده شود تا از Channel

### 9.7 تغییر Chat.razor برای چت چندنفره

**تغییرات در Chat.razor:**
```razor
@code {
    [Parameter]
    public string CurrentUser { get; set; }
    
    [Parameter]
    public string ChatId { get; set; } // جدید
    
    [Parameter]
    public string ReceiverUsername { get; set; } // برای چت 1-to-1
    
    private List<MessageModel> _messages;
    
    protected override void OnInitialized()
    {
        ChatService.MessageReceived += OnMessage;
        _messages = new List<MessageModel>();
        
        // بارگذاری تاریخچه پیام‌ها
        if (!string.IsNullOrEmpty(ChatId))
        {
            LoadChatHistory(ChatId);
        }
        else if (!string.IsNullOrEmpty(ReceiverUsername))
        {
            LoadPrivateChatHistory(CurrentUser, ReceiverUsername);
        }
    }
    
    private void OnMessage(object sender, MessageModel message)
    {
        // فیلتر کردن پیام‌ها
        bool shouldShow = false;
        
        if (!string.IsNullOrEmpty(ChatId))
        {
            // چت گروهی
            shouldShow = message.ChatId == ChatId;
        }
        else if (!string.IsNullOrEmpty(ReceiverUsername))
        {
            // چت خصوصی
            shouldShow = (message.Username == CurrentUser && message.ReceiverUsername == ReceiverUsername) ||
                        (message.Username == ReceiverUsername && message.ReceiverUsername == CurrentUser);
        }
        else
        {
            // پیام عمومی
            shouldShow = string.IsNullOrEmpty(message.ReceiverUsername) && string.IsNullOrEmpty(message.ChatId);
        }
        
        if (shouldShow)
        {
            _messages.Add(message);
            InvokeAsync(StateHasChanged);
            ScrollToBottom();
        }
    }
    
    private void LoadChatHistory(string chatId)
    {
        var messages = ChatService.GetMessagesForChat(chatId, CurrentUser);
        _messages = messages.ToList();
    }
}
```

### 9.8 MultiChatView Component (جزئیات کامل)

**ساختار:**
```razor
<div class="multi-chat-container">
    <div class="chat-list-panel">
        <div class="chat-list-header">
            <h3>چت‌ها</h3>
            <button @onclick="ShowCreateChatModal">+ چت جدید</button>
        </div>
        <div class="chat-list">
            @foreach (var chat in _chatThreads)
            {
                <div class="chat-item @(chat.ChatId == SelectedChatId ? "active" : "")" 
                     @onclick="() => SelectChat(chat.ChatId)">
                    <div class="chat-avatar">@GetChatInitials(chat)</div>
                    <div class="chat-info">
                        <div class="chat-name">@GetChatName(chat)</div>
                        <div class="chat-preview">@chat.LastMessagePreview</div>
                    </div>
                    @if (chat.UnreadCounts.ContainsKey(CurrentUser) && chat.UnreadCounts[CurrentUser] > 0)
                    {
                        <span class="unread-badge">@chat.UnreadCounts[CurrentUser]</span>
                    }
                </div>
            }
        </div>
    </div>
    
    <div class="chat-content-panel">
        @if (!string.IsNullOrEmpty(SelectedChatId))
        {
            <Chat CurrentUser="@CurrentUser" ChatId="@SelectedChatId" />
            <MessageForm LoggedUser="@LoggedUser" ChatId="@SelectedChatId" />
        }
        else
        {
            <div class="no-chat-selected">
                <p>یک چت را انتخاب کنید</p>
            </div>
        }
    </div>
</div>
```

---

## Part 10: جزئیات صفحات راهنما و مستندات

### 10.1 ساختار صفحه Help

**بخش‌های اصلی:**
1. **شروع کار**
   - اسکن QR Code
   - ورود به سیستم
   - پیدا کردن کد کاربری

2. **ارسال پیام**
   - ارسال پیام عمومی (به همه)
   - ارسال پیام خصوصی
   - پاسخ به پیام

3. **چت گروهی**
   - ایجاد چت گروهی
   - اضافه کردن اعضا
   - مدیریت چت

4. **تنظیمات**
   - تغییر کد کاربری
   - خروج از حساب

5. **FAQ**
   - سوالات متداول با پاسخ

6. **عیب‌یابی**
   - مشکلات رایج و راه حل

### 10.2 محتوای صفحه API Documentation

**بخش‌های اضافی:**
- **Authentication**: توضیح درباره AllowAnonymous و آینده (API Key)
- **Rate Limiting**: محدودیت‌های فعلی و آینده
- **Webhooks**: (آینده) امکان دریافت webhook برای پیام‌ها
- **SDKs**: لینک به SDKs مختلف (اگر در آینده ایجاد شوند)
- **Changelog**: تاریخچه تغییرات API
- **Error Codes**: لیست کامل کدهای خطا

### 10.3 بهبود ChatController

**اضافه کردن:**
```csharp
/// <summary>
/// ارسال پیام از طریق API
/// </summary>
/// <param name="model">اطلاعات پیام</param>
/// <returns>نتیجه ارسال</returns>
/// <response code="200">پیام با موفقیت ارسال شد</response>
/// <response code="400">اطلاعات ورودی نامعتبر است</response>
/// <response code="500">خطای سرور</response>
[HttpPost]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> PostMessageAsync([FromBody] SendMessageDtoModel model)
{
    // Validation
    if (string.IsNullOrWhiteSpace(model.Username))
        return BadRequest(new ApiError { Message = "Username is required" });
    
    if (string.IsNullOrWhiteSpace(model.Message))
        return BadRequest(new ApiError { Message = "Message is required" });
    
    try
    {
        await _publisher.PublishAsync(new MessageModel(
            "api", 
            model.ReceiverUsername, 
            model.Message, 
            DateTime.UtcNow));
        
        return Ok(new ApiResponse 
        { 
            Success = true, 
            Message = "Message sent successfully" 
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new ApiError { Message = ex.Message });
    }
}
```

**مدل‌های Response:**
```csharp
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}

public class ApiError
{
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}
```

---

## Part 11: بهبودهای UX/UI

### 11.1 Toast Notifications
- نمایش notification برای پیام‌های جدید
- نمایش notification برای اخطارها

### 11.2 Loading States
- Skeleton loaders برای چت‌ها
- Loading indicators برای ارسال پیام

### 11.3 Keyboard Shortcuts
- Enter برای ارسال پیام
- Ctrl+K برای جستجوی چت
- Escape برای بستن modals

---

## Part 12: Testing Plan

### 12.1 Unit Tests
- ChatThreadService tests
- MessageModel tests
- ChatService tests

### 12.2 Integration Tests
- API endpoint tests
- Multi-chat functionality tests

### 12.3 E2E Tests (Manual)
- تست کامل جریان چت چندنفره
- تست صفحات جدید
- تست responsive design

---

## Timeline پیشنهادی

### Week 1: زیرساخت
- Day 1-2: ایجاد Models و Services برای چت چندنفره
- Day 3-4: تغییر ChatService و MessageModel
- Day 5: تست و debug

### Week 2: UI برای چت چندنفره
- Day 1-2: ایجاد MultiChatView component
- Day 3-4: تغییر Chat.razor و MessageForm
- Day 5: تست و بهبود

### Week 3: صفحات مستندات
- Day 1-2: ایجاد صفحات Help, About, Contact
- Day 3-4: ایجاد صفحه API Docs
- Day 5: Navigation و بهبود UI

### Week 4: Polish و Testing
- Day 1-2: بهبود UX/UI
- Day 3-4: Testing کامل
- Day 5: Bug fixes و documentation

---

## Risks و Mitigation

### Risk 1: Performance در چت‌های بزرگ
**Mitigation:** محدود کردن تعداد پیام‌های نمایش داده شده (pagination)

### Risk 2: Memory Leak در ذخیره‌سازی پیام‌ها
**Mitigation:** محدود کردن تعداد پیام‌های ذخیره شده در memory

### Risk 3: پیچیدگی مدیریت state
**Mitigation:** استفاده از State Management pattern مناسب

---

## Dependencies

### Required:
- .NET 9 SDK (موجود ✓)
- Blazor Server (موجود ✓)

### Optional (برای آینده):
- Database برای ذخیره پیام‌ها (SQL Server, PostgreSQL)
- Redis برای caching
- SignalR Hub برای real-time communication بهتر

---

## Success Criteria

1. ✅ کاربران می‌توانند چت گروهی ایجاد کنند
2. ✅ کاربران می‌توانند همزمان در چند چت شرکت کنند
3. ✅ صفحات Help, About, Contact, API Docs قابل دسترسی هستند
4. ✅ Navigation کامل است
5. ✅ API مستندسازی شده است
6. ✅ UI/UX بهبود یافته است
7. ✅ تمام تست‌ها pass می‌شوند

---

**این plan کامل و آماده پیاده‌سازی است. تمام جزئیات مورد نیاز برای شروع کار در دسترس است.**

---

## Part 13: پیاده‌سازی عملی - Phase 1

### 13.1 ایجاد صفحات پایه (اولویت اول)

#### Step 1: ایجاد صفحه About
**اولویت:** بالا - ساده‌ترین صفحه

#### Step 2: ایجاد صفحه Contact  
**اولویت:** بالا - نیاز به فرم ساده

#### Step 3: ایجاد صفحه Help
**اولویت:** متوسط - نیاز به محتوای بیشتر

#### Step 4: ایجاد صفحه ApiDocs
**اولویت:** متوسط - نیاز به مثال‌های کد

#### Step 5: بهبود Navigation در MainLayout
**اولویت:** بالا - برای دسترسی به صفحات جدید

---

## Part 14: جزئیات پیاده‌سازی چت چندنفره (تکمیلی)

### 14.1 مشکل ذخیره‌سازی پیام‌ها - راه حل عملی

**تحلیل:** 
- فعلاً `MessagesConsumer` پیام‌ها را فقط broadcast می‌کند
- برای چت‌های چندنفره نیاز به تاریخچه داریم
- Channel فقط برای real-time messaging است

**راه حل پیشنهادی (MVP):**
1. ایجاد `MessageRepository` با In-Memory storage
2. ذخیره آخرین 1000 پیام برای هر چت
3. استفاده از `ConcurrentDictionary<string, List<MessageModel>>`
4. Key: ChatId یا PrivateChatKey

**کد MessageRepository:**
```csharp
public interface IMessageRepository
{
    void AddMessage(MessageModel message);
    IEnumerable<MessageModel> GetMessages(string chatId, int limit = 100);
    IEnumerable<MessageModel> GetPrivateChatMessages(string user1, string user2, int limit = 100);
    int GetUnreadCount(string chatId, string username);
    void MarkAsRead(string chatId, string username);
}

public class InMemoryMessageRepository : IMessageRepository
{
    private readonly ConcurrentDictionary<string, List<MessageModel>> _chatMessages;
    private readonly ConcurrentDictionary<string, HashSet<string>> _readMessages; // chatId -> usernames who read
    
    public InMemoryMessageRepository()
    {
        _chatMessages = new ConcurrentDictionary<string, List<MessageModel>>();
        _readMessages = new ConcurrentDictionary<string, HashSet<string>>();
    }
    
    public void AddMessage(MessageModel message)
    {
        string key = GetMessageKey(message);
        
        _chatMessages.AddOrUpdate(key,
            new List<MessageModel> { message },
            (k, list) =>
            {
                list.Add(message);
                // محدود کردن به 1000 پیام
                if (list.Count > 1000)
                    list.RemoveAt(0);
                return list;
            });
    }
    
    private string GetMessageKey(MessageModel message)
    {
        if (!string.IsNullOrEmpty(message.ChatId))
            return $"chat_{message.ChatId}";
        
        if (!string.IsNullOrEmpty(message.ReceiverUsername))
        {
            // برای چت خصوصی، key باید همیشه یکسان باشد (user1_user2 یا user2_user1)
            var users = new[] { message.Username, message.ReceiverUsername }.OrderBy(u => u);
            return $"private_{string.Join("_", users)}";
        }
        
        return "public";
    }
    
    public IEnumerable<MessageModel> GetMessages(string chatId, int limit = 100)
    {
        string key = $"chat_{chatId}";
        if (_chatMessages.TryGetValue(key, out var messages))
        {
            return messages.TakeLast(limit);
        }
        return Enumerable.Empty<MessageModel>();
    }
    
    public IEnumerable<MessageModel> GetPrivateChatMessages(string user1, string user2, int limit = 100)
    {
        var users = new[] { user1, user2 }.OrderBy(u => u);
        string key = $"private_{string.Join("_", users)}";
        
        if (_chatMessages.TryGetValue(key, out var messages))
        {
            return messages.TakeLast(limit);
        }
        return Enumerable.Empty<MessageModel>();
    }
    
    public int GetUnreadCount(string chatId, string username)
    {
        string key = $"chat_{chatId}";
        if (!_chatMessages.TryGetValue(key, out var messages))
            return 0;
        
        string readKey = $"{key}_{username}";
        if (!_readMessages.TryGetValue(readKey, out var readSet))
            return messages.Count;
        
        // تعداد پیام‌هایی که بعد از آخرین خوانده شده، ارسال شده‌اند
        return messages.Count(m => !readSet.Contains(GetMessageId(m)));
    }
    
    private string GetMessageId(MessageModel message)
    {
        // از timestamp و username برای ایجاد unique ID استفاده می‌کنیم
        return $"{message.Username}_{message.When:yyyyMMddHHmmssfff}";
    }
}
```

### 14.2 اتصال MessageRepository به MessagesConsumer

**تغییر MessagesConsumerWorker:**
```csharp
public class MessagesConsumerWorker : BackgroundService
{
    private readonly IMessagesConsumer _consumer;
    private readonly IMessageRepository _messageRepository;
    
    public MessagesConsumerWorker(IMessagesConsumer consumer, IMessageRepository messageRepository)
    {
        _consumer = consumer;
        _messageRepository = messageRepository;
        _consumer.MessageReceived += OnMessageReceived;
    }
    
    private void OnMessageReceived(object sender, MessageModel message)
    {
        // ذخیره پیام در repository
        _messageRepository.AddMessage(message);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.BeginConsumeAsync();
    }
}
```

### 14.3 تغییر ChatService برای استفاده از MessageRepository

```csharp
public class ChatService : IChatService
{
    private readonly IUserStateProvider _usersProvider;
    private readonly IMessagesPublisher _publisher;
    private readonly IMessagesConsumer _consumer;
    private readonly IChatThreadService _chatThreadService; // جدید
    private readonly IMessageRepository _messageRepository; // جدید
    
    public ChatService(
        IUserStateProvider usersProvider, 
        IMessagesPublisher publisher, 
        IMessagesConsumer consumer,
        IChatThreadService chatThreadService,
        IMessageRepository messageRepository)
    {
        _usersProvider = usersProvider;
        _publisher = publisher;
        _consumer = consumer;
        _chatThreadService = chatThreadService;
        _messageRepository = messageRepository;
        _consumer.MessageReceived += OnMessage;
    }
    
    // متدهای جدید برای چت گروهی
    public ChatThreadModel CreateGroupChat(List<string> participants, string createdBy, string chatName = null)
    {
        if (participants == null || participants.Count < 2)
            throw new ArgumentException("At least 2 participants required");
        
        if (!participants.Contains(createdBy))
            participants.Add(createdBy);
        
        return _chatThreadService.CreateChatThread(participants, createdBy, chatName);
    }
    
    public async Task PostMessageToChatAsync(string chatId, UserModel user, string message)
    {
        var chatThread = _chatThreadService.GetChatThread(chatId);
        if (chatThread == null || !chatThread.IsParticipant(user.Username))
            throw new UnauthorizedAccessException("User is not a participant");
        
        var msg = new MessageModel(user.Username, chatId, message, DateTime.UtcNow, true);
        await _publisher.PublishAsync(msg);
        
        _chatThreadService.UpdateLastMessage(chatId, msg.When, message);
        
        foreach (var participant in chatThread.Participants)
        {
            if (participant != user.Username)
                _chatThreadService.IncrementUnreadCount(chatId, participant);
        }
    }
    
    public IEnumerable<MessageModel> GetMessagesForChat(string chatId, string username)
    {
        var chatThread = _chatThreadService.GetChatThread(chatId);
        if (chatThread == null || !chatThread.IsParticipant(username))
            return Enumerable.Empty<MessageModel>();
        
        return _messageRepository.GetMessages(chatId, 100);
    }
    
    public IEnumerable<ChatThreadModel> GetChatThreadsForUser(string username)
    {
        return _chatThreadService.GetChatThreadsForUser(username);
    }
}
```

### 14.4 تغییر Chat.razor برای بارگذاری تاریخچه

```csharp
protected override void OnInitialized()
{
    ChatService.MessageReceived += OnMessage;
    _messages = new List<MessageModel>();
    
    // بارگذاری تاریخچه
    LoadChatHistory();
}

private void LoadChatHistory()
{
    if (!string.IsNullOrEmpty(ChatId))
    {
        var history = ChatService.GetMessagesForChat(ChatId, CurrentUser);
        _messages = history.ToList();
    }
    else if (!string.IsNullOrEmpty(ReceiverUsername))
    {
        var history = ChatService.GetPrivateChatMessages(CurrentUser, ReceiverUsername);
        _messages = history.ToList();
    }
}
```

---

## Part 15: ثبت Services در Startup.cs

### تغییرات مورد نیاز:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ... کدهای موجود ...
    
    // اضافه کردن MessageRepository
    services.AddSingleton<IMessageRepository, InMemoryMessageRepository>();
    
    // اضافه کردن ChatThreadService
    services.AddSingleton<IChatThreadService, ChatThreadService>();
    
    // ... بقیه کدها ...
}
```

---

## Part 16: Checklist پیاده‌سازی

### Phase 1: صفحات پایه (هفته 1)
- [ ] ایجاد About.razor
- [ ] ایجاد Contact.razor  
- [ ] ایجاد Help.razor
- [ ] ایجاد ApiDocs.razor
- [ ] بهبود MainLayout.razor با Navigation
- [ ] تست routing

### Phase 2: زیرساخت چت چندنفره (هفته 2)
- [ ] ایجاد ChatThreadModel.cs
- [ ] ایجاد IChatThreadService.cs
- [ ] ایجاد ChatThreadService.cs
- [ ] ایجاد IMessageRepository.cs
- [ ] ایجاد InMemoryMessageRepository.cs
- [ ] تغییر MessageModel.cs (اضافه کردن ChatId)
- [ ] ثبت Services در Startup.cs
- [ ] تست Services

### Phase 3: تغییرات ChatService (هفته 2)
- [ ] تغییر ChatService برای چت گروهی
- [ ] اضافه کردن متد CreateGroupChat
- [ ] اضافه کردن متد PostMessageToChatAsync
- [ ] اضافه کردن متد GetMessagesForChat
- [ ] اضافه کردن متد GetChatThreadsForUser
- [ ] تست ChatService

### Phase 4: تغییرات UI Components (هفته 3)
- [ ] تغییر Chat.razor برای ChatId
- [ ] تغییر MessageForm.razor برای ChatId
- [ ] تغییر Users.razor برای انتخاب چند کاربر
- [ ] ایجاد MultiChatView.razor
- [ ] تغییر Index.razor
- [ ] تست UI

### Phase 5: Polish و Testing (هفته 4)
- [ ] بهبود UX/UI
- [ ] اضافه کردن Loading states
- [ ] تست کامل functionality
- [ ] Bug fixes
- [ ] Documentation

---

## نکات مهم پیاده‌سازی:

1. **Backward Compatibility**: تمام تغییرات باید با کد فعلی سازگار باشند
2. **Test هر بخش**: بعد از هر تغییر، تست کنید
3. **Incremental Development**: تغییرات را مرحله به مرحله انجام دهید
4. **Error Handling**: برای تمام متدهای جدید error handling اضافه کنید
5. **Logging**: برای debugging، logging اضافه کنید (اگر نیاز است)

---

**Plan اکنون کامل است و آماده شروع پیاده‌سازی. می‌توانید از Phase 1 شروع کنید.**
