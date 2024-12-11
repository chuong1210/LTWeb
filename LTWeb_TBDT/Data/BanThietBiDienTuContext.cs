using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LTWeb_TBDT.Data
{
    public partial class BanThietBiDienTuContext : DbContext
    {
        public BanThietBiDienTuContext()
        {
        }

        public BanThietBiDienTuContext(DbContextOptions<BanThietBiDienTuContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; } = null!;
        public virtual DbSet<DanhMuc> DanhMucs { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public virtual DbSet<NhaSanXuat> NhaSanXuats { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<SanPham> SanPhams { get; set; } = null!;
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public virtual DbSet<TonKho> TonKhos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=USER\\MSSQLSERVER01;Database=BanThietBiDienTu;User ID=sa;Password=101204;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.MaAdmin)
                    .HasName("PK__Admin__49341E38A8E7E2E7");

                entity.ToTable("Admin");

                entity.HasOne(d => d.MaTaiKhoanNavigation)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.MaTaiKhoan)
                    .HasConstraintName("FK__Admin__MaTaiKhoa__3C69FB99");
            });

            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                entity.HasKey(e => new { e.MaHoaDon, e.MaSanPham })
                    .HasName("PK__ChiTietH__4CF2A579F10FFA81");

                entity.ToTable("ChiTietHoaDon");

                entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ThanhTien)
                    .HasColumnType("decimal(29, 2)")
                    .HasComputedColumnSql("([SoLuong]*[GiaBan])", false);

                entity.HasOne(d => d.MaHoaDonNavigation)
                    .WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaHoaDon)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietHo__MaHoa__4CA06362");

                entity.HasOne(d => d.MaSanPhamNavigation)
                    .WithMany(p => p.ChiTietHoaDons)
                    .HasForeignKey(d => d.MaSanPham)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietHo__MaSan__4D94879B");
            });

            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.HasKey(e => e.MaDanhMuc)
                    .HasName("PK__DanhMuc__B3750887D356A157");

                entity.ToTable("DanhMuc");

                entity.Property(e => e.TenDanhMuc).HasMaxLength(255);
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHoaDon)
                    .HasName("PK_HOADON");

                entity.ToTable("HoaDon");

                entity.Property(e => e.NgayDatHang).HasColumnType("date");

                entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaKhachHangNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaKhachHang)
                    .HasConstraintName("FK__HoaDon__MaKhachH__49C3F6B7");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKhachHang)
                    .HasName("PK__KhachHan__88D2F0E5672AA59C");

                entity.ToTable("KhachHang");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.SoDienThoai).HasMaxLength(15);

                entity.HasOne(d => d.MaTaiKhoanNavigation)
                    .WithMany(p => p.KhachHangs)
                    .HasForeignKey(d => d.MaTaiKhoan)
                    .HasConstraintName("FK__KhachHang__MaTai__3F466844");
            });

            modelBuilder.Entity<NhaSanXuat>(entity =>
            {
                entity.HasKey(e => e.MaNhaSanXuat)
                    .HasName("PK__NhaSanXu__838C17A152F203E4");

                entity.ToTable("NhaSanXuat");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.SoDienThoai).HasMaxLength(15);

                entity.Property(e => e.TenNhaSanXuat).HasMaxLength(255);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNhanVien)
                    .HasName("PK__NhanVien__77B2CA4700946649");

                entity.ToTable("NhanVien");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.SoDienThoai).HasMaxLength(15);

                entity.HasOne(d => d.MaTaiKhoanNavigation)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.MaTaiKhoan)
                    .HasConstraintName("FK__NhanVien__MaTaiK__398D8EEE");
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.MaSanPham)
                    .HasName("PK__SanPham__FAC7442D2304C06A");

                entity.ToTable("SanPham");

                entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TenSanPham).HasMaxLength(255);

                entity.HasOne(d => d.MaDanhMucNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaDanhMuc)
                    .HasConstraintName("FK__SanPham__MaDanhM__46E78A0C");

                entity.HasOne(d => d.MaNhaSanXuatNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaNhaSanXuat)
                    .HasConstraintName("FK__SanPham__MaNhaSa__45F365D3");
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.MaTaiKhoan)
                    .HasName("PK__TaiKhoan__AD7C652988F82420");

                entity.ToTable("TaiKhoan");

                entity.Property(e => e.LoaiTaiKhoan).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(255);

                entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            });

            modelBuilder.Entity<TonKho>(entity =>
            {
                entity.HasKey(e => e.MaSanPham)
                    .HasName("PK__TonKho__FAC7442D8EEE2EA2");

                entity.ToTable("TonKho");

                entity.Property(e => e.MaSanPham).ValueGeneratedNever();

                entity.HasOne(d => d.MaSanPhamNavigation)
                    .WithOne(p => p.TonKho)
                    .HasForeignKey<TonKho>(d => d.MaSanPham)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TonKho__MaSanPha__5070F446");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
