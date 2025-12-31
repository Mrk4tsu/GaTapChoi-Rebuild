# Gà Tập Chơi - Rebuild
![](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)
### Giới thiệu
Dự án GaTapChoi-Rebuild là phiên bản rebuild của backend API cho website [Gà Tập Chơi](https://gavl.io.vn/ "Gà Tập Chơi"), một nền tảng dành cho cộng đồng game thủ Việt Nam. Website tập trung vào việc chia sẻ mod game (đặc biệt cho Ngọc Rồng Online và các game khác), tin tức game nóng hổi, tải game, và tùy chỉnh nhân vật (cải trang/skins) với giá cả phải chăng.
Backend này xử lý các tính năng cốt lõi như quản lý mod, đăng nhập người dùng, upload file, caching dữ liệu, và tích hợp cơ sở dữ liệu. Dự án được xây dựng với kiến trúc phân tầng để dễ bảo trì và mở rộng.

### Tính năng chính
- **Quản lý Mod**: Tạo, cập nhật, xóa, và lấy danh sách mod với hỗ trợ phân trang, thumbnail, và trạng thái.
- **Xác thực Người dùng**: Sử dụng JWT cho đăng nhập, đăng ký, khôi phục mật khẩu, và quản lý vai trò (roles).
- **Quản lý Bài Đăng**: Tạo bài đăng với upload file lên Cloudflare R2.
- **Caching**: Sử dụng Redis để tối ưu hiệu suất, bao gồm cache mod và token.
- **Cơ sở dữ liệu**: PostgreSQL với migration hỗ trợ UUIDv7 cho ID.
- **Tích hợp**: Hỗ trợ internal endpoints, time handling, và URL management.
- **Deployment**: Tích hợp GitHub Actions và Azure App Service cho CI/CD.

### Công nghệ sử dụng
- Ngôn ngữ: C# (.NET)
- Database: PostgreSQL (chuyển từ SQL Server)
- Caching: Redis
- Authentication: JWT
- Storage: Cloudflare R2 cho file upload
- Công cụ: GitHub Actions cho workflow, Visual Studio cho development

### Cấu trúc dự án
Dự án là một solution .NET với các project phân tầng:
- **GaVL.API**: Xử lý các endpoint API và controller.
- **GaVL.Application**: Logic business và services (ví dụ: ModService, AuthService).
- **GaVL.DTO**: Data Transfer Objects cho dữ liệu trao đổi.
- **GaVL.Data**: Layer data access với Entity Framework và migrations.
- **GaVL.Utilities**: Các helper như TimeHelper, URL management.

### Hướng dẫn cài đặt
#### Yêu cầu
- .NET SDK 8.0 hoặc cao hơn
- PostgreSQL server
- Redis server
- Tài khoản Cloudflare R2 (cho file upload)
- Visual Studio 2022 (khuyến nghị)

#### Các bước cài đặt
1. Clone repository:
```bash
clone https://github.com/Mrk4tsu/GaTapChoi-Rebuild.git
```
2. Mở file `GaVL.sln` trong Visual Studio.
3. Cấu hình connection strings trong `appsettings.json` (trong GaVL.API):
	- PostgreSQL: `ConnectionStrings:DefaultConnection`
	- Redis: `Redis:ConnectionString`
4. Chạy migrations để tạo database
```bash
add-migration Initial
```
```bash
update-database
```
5. Build và run project:
Chạy GaVL.API như project startup.
6. Để deploy lên Azure:
Sử dụng workflow trong `.github/workflows` để build và deploy tự động.


#### Sử dụng
Các endpoint chính (ví dụ với base URL: https://your-api-url)

Sử dụng Postman hoặc tương tự để test API. Token JWT cần được gửi trong header `Authorization: Bearer {token}` cho các endpoint bảo vệ.
Đóng góp
Chúng tôi hoan nghênh đóng góp từ cộng đồng! Để đóng góp:
- Fork repository.
- Tạo branch mới: git checkout -b feature/ten-tinh-nang.
- Commit thay đổi: git commit -m 'Mô tả thay đổi'.
- Push branch: git push origin feature/ten-tinh-nang.
- Tạo Pull Request.

Vui lòng tuân thủ code style và thêm test nếu có thể.
### Giấy phép
Dự án được cấp phép dưới GNU Affero General Public License v3.0 (AGPL-3.0). Bạn có thể sử dụng, sửa đổi, và phân phối miễn là tuân thủ các điều khoản của giấy phép, bao gồm việc công khai source code nếu deploy công khai.
### Liên hệ
Owner: [Mrk4tsu](https://github.com/Mrk4tsu "Mrk4tsu")
Website: [gavl.io.vn](https://gavl.io.vn/ "gavl.io.vn")
Nếu có vấn đề, mở issue trên GitHub.

Cảm ơn bạn đã quan tâm đến dự án!
