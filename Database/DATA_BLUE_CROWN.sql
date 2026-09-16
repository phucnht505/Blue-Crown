USE BLUE_CROWN;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- =====================================================
    -- 0. XÓA DỮ LIỆU CŨ THEO THỨ TỰ KHÓA NGOẠI
    -- =====================================================
    DELETE FROM receipt_details;
    DELETE FROM inventory_receipts;

    DELETE FROM order_items;
    DELETE FROM ecommerce_orders;

    DELETE FROM drug_alternatives;
    DELETE FROM auto_prescriptions;
    DELETE FROM symptom_logs;

    DELETE FROM notifications;
    DELETE FROM payments;
    DELETE FROM health_goals;

    DELETE FROM prescription_dispense_items;
    DELETE FROM prescription_items;
    DELETE FROM prescriptions;

    DELETE FROM medical_records;
    DELETE FROM appointments;

    DELETE FROM chat_messages;
    DELETE FROM chat_sessions;

    DELETE FROM health_metrics;

    DELETE FROM doctor_profiles;
    DELETE FROM patient_profiles;

    DELETE FROM medications;
    DELETE FROM metric_types;

    DELETE FROM clinics;
    DELETE FROM suppliers;
    DELETE FROM products;

    DELETE FROM users;

    DBCC CHECKIDENT ('metric_types', RESEED, 0);

    -- =====================================================
    -- 1. USERS
    -- Mật khẩu tất cả tài khoản: BlueCrown@123
    -- BCrypt hash tương thích BCrypt.Net
    -- =====================================================
    DECLARE @PasswordHash NVARCHAR(255) =
        N'$2y$12$8XHxIMNCvtGl2Tw8yWgmkebH5.9/aVPyf9fQflXVVEcqUwAMvjZ5W';

    INSERT INTO users
    (
        id, email, phone, password_hash, full_name,
        date_of_birth, gender, avatar_url,
        role, status, email_verified_at, created_at, updated_at
    )
    VALUES
    -- Patients
    ('00000000-0000-0000-0000-000000001001', N'an.nguyen@gmail.com',  N'0901001001', @PasswordHash, N'Nguyễn Văn An',
     '1998-05-12', N'male', NULL, N'patient', N'active', '2026-01-10T08:00:00', '2026-01-05T09:00:00', '2026-01-10T08:00:00'),

    ('00000000-0000-0000-0000-000000001002', N'binh.tran@gmail.com',  N'0901001002', @PasswordHash, N'Trần Thị Bình',
     '1995-11-23', N'female', NULL, N'patient', N'active', '2026-01-12T08:00:00', '2026-01-06T09:00:00', '2026-01-12T08:00:00'),

    ('00000000-0000-0000-0000-000000001003', N'cuong.le@gmail.com',   N'0901001003', @PasswordHash, N'Lê Hoàng Cường',
     '2000-02-28', N'male', NULL, N'patient', N'active', '2026-01-15T08:00:00', '2026-01-07T09:00:00', '2026-01-15T08:00:00'),

    ('00000000-0000-0000-0000-000000001004', N'dung.pham@gmail.com', N'0901001004', @PasswordHash, N'Phạm Thị Dung',
     '1988-07-19', N'female', NULL, N'patient', N'active', '2026-01-18T08:00:00', '2026-01-08T09:00:00', '2026-01-18T08:00:00'),

    ('00000000-0000-0000-0000-000000001005', N'em.dang@gmail.com',    N'0901001005', @PasswordHash, N'Đặng Thị Em',
     '2002-09-03', N'female', NULL, N'patient', N'pending', NULL, '2026-02-01T09:00:00', '2026-02-01T09:00:00'),

    -- Doctors
    ('00000000-0000-0000-0000-000000002001', N'tuan.do@bluecrown.vn', N'0912002001', @PasswordHash, N'BS. Đỗ Minh Tuấn',
     '1980-03-15', N'male', NULL, N'doctor', N'active', '2025-11-01T08:00:00', '2025-10-20T09:00:00', '2025-11-01T08:00:00'),

    ('00000000-0000-0000-0000-000000002002', N'hoa.vu@bluecrown.vn', N'0912002002', @PasswordHash, N'BS. Vũ Thị Hoa',
     '1983-06-22', N'female', NULL, N'doctor', N'active', '2025-11-05T08:00:00', '2025-10-22T09:00:00', '2025-11-05T08:00:00'),

    ('00000000-0000-0000-0000-000000002003', N'long.bui@bluecrown.vn', N'0912002003', @PasswordHash, N'BS. Bùi Văn Long',
     '1978-12-01', N'male', NULL, N'doctor', N'active', '2025-11-10T08:00:00', '2025-10-25T09:00:00', '2025-11-10T08:00:00'),

    -- Admin
    ('00000000-0000-0000-0000-000000003001', N'admin@bluecrown.vn', N'0923003001', @PasswordHash, N'Quản trị viên Blue Crown',
     '1990-04-05', N'female', NULL, N'admin', N'active', '2025-09-01T08:00:00', '2025-09-01T08:00:00', '2025-09-01T08:00:00'),

    -- Pharmacists
    ('00000000-0000-0000-0000-000000004001', N'phuc.ngo@bluecrown.vn', N'0934004001', @PasswordHash, N'DS. Ngô Văn Phúc',
     '1992-08-17', N'male', NULL, N'pharmacist', N'active', '2025-09-15T08:00:00', '2025-09-10T08:00:00', '2025-09-15T08:00:00'),

    ('00000000-0000-0000-0000-000000004002', N'lan.vo@bluecrown.vn', N'0934004002', @PasswordHash, N'DS. Võ Thị Lan',
     '1994-02-10', N'female', NULL, N'pharmacist', N'active', '2025-09-18T08:00:00', '2025-09-12T08:00:00', '2025-09-18T08:00:00');

    -- =====================================================
    -- 2. PATIENT PROFILES
    -- =====================================================
    INSERT INTO patient_profiles
    (
        id, user_id, blood_type, height_cm, WeightKg,
        allergies, chronic_conditions,
        emergency_contact_name, emergency_contact_phone
    )
    VALUES
    ('00000000-0000-0000-0000-000000011001', '00000000-0000-0000-0000-000000001001', N'O+',  172.50, 68.00, N'Không', N'Không', N'Nguyễn Văn Bảo', N'0987001001'),
    ('00000000-0000-0000-0000-000000011002', '00000000-0000-0000-0000-000000001002', N'A+',  160.00, 54.50, N'Dị ứng Penicillin', N'Tăng huyết áp', N'Trần Văn Cảnh', N'0987001002'),
    ('00000000-0000-0000-0000-000000011003', '00000000-0000-0000-0000-000000001003', N'B+',  175.00, 72.00, N'Không', N'Không', N'Lê Thị Duyên', N'0987001003'),
    ('00000000-0000-0000-0000-000000011004', '00000000-0000-0000-0000-000000001004', N'AB+', 158.00, 60.00, N'Dị ứng hải sản', N'Đái tháo đường type 2', N'Phạm Văn Giang', N'0987001004'),
    ('00000000-0000-0000-0000-000000011005', '00000000-0000-0000-0000-000000001005', N'O-',  165.00, 57.00, N'Không', N'Không', N'Đặng Văn Hùng', N'0987001005');

    -- =====================================================
    -- 3. CLINICS
    -- =====================================================
    INSERT INTO clinics (id, name, address, phone)
    VALUES
    ('00000000-0000-0000-0000-000000021001', N'Phòng khám Đa khoa Blue Crown Quận 1', N'123 Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM', N'02838221001'),
    ('00000000-0000-0000-0000-000000021002', N'Phòng khám Nhi & Da liễu Blue Crown Quận 3', N'45 Võ Văn Tần, Phường 6, Quận 3, TP.HCM', N'02838221002'),
    ('00000000-0000-0000-0000-000000021003', N'Phòng khám Nội khoa Blue Crown Bình Thạnh', N'80 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM', N'02838221003');

    -- =====================================================
    -- 4. DOCTOR PROFILES
    -- =====================================================
    INSERT INTO doctor_profiles
    (
        id, user_id, specialty, license_number, license_verified,
        bio, years_experience, clinic_id,
        consultation_fee, rating_avg, rating_count
    )
    VALUES
    ('00000000-0000-0000-0000-000000031001', '00000000-0000-0000-0000-000000002001',
     N'Tim mạch', N'VN-CARD-2015-0012', 1,
     N'Bác sĩ chuyên khoa tim mạch, có kinh nghiệm theo dõi tăng huyết áp và bệnh mạch vành.',
     15, '00000000-0000-0000-0000-000000021001', 300000, 4.80, 152),

    ('00000000-0000-0000-0000-000000031002', '00000000-0000-0000-0000-000000002002',
     N'Nhi khoa', N'VN-PED-2017-0048', 1,
     N'Bác sĩ nhi khoa, tư vấn các bệnh hô hấp và chăm sóc sức khỏe trẻ em.',
     12, '00000000-0000-0000-0000-000000021002', 250000, 4.90, 210),

    ('00000000-0000-0000-0000-000000031003', '00000000-0000-0000-0000-000000002003',
     N'Nội tổng quát', N'VN-INT-2012-0105', 1,
     N'Bác sĩ nội tổng quát, theo dõi bệnh mạn tính và tư vấn sức khỏe người trưởng thành.',
     18, '00000000-0000-0000-0000-000000021003', 280000, 4.70, 126);

    -- =====================================================
    -- 5. METRIC TYPES
    -- =====================================================
    SET IDENTITY_INSERT metric_types ON;

    INSERT INTO metric_types (id, code, name, unit, normal_min, normal_max)
    VALUES
    (1, N'WEIGHT', N'Cân nặng', N'kg', NULL, NULL),
    (2, N'HEART_RATE', N'Nhịp tim', N'bpm', 60, 100),
    (3, N'SYSTOLIC_BP', N'Huyết áp tâm thu', N'mmHg', 90, 120),
    (4, N'DIASTOLIC_BP', N'Huyết áp tâm trương', N'mmHg', 60, 80),
    (5, N'BLOOD_GLUCOSE', N'Đường huyết', N'mg/dL', 70, 99),
    (6, N'SPO2', N'Nồng độ oxy máu', N'%', 95, 100);

    SET IDENTITY_INSERT metric_types OFF;

    -- =====================================================
    -- 6. HEALTH METRICS
    -- =====================================================
    INSERT INTO health_metrics (id, patient_id, metric_type_id, value, recorded_at)
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000011001', 1, 68.00, '2026-08-01T07:30:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011001', 2, 76.00, '2026-08-01T07:35:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011001', 6, 98.00, '2026-08-01T07:36:00'),

    (NEWID(), '00000000-0000-0000-0000-000000011002', 1, 54.50, '2026-08-02T07:30:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011002', 3, 135.00, '2026-08-02T07:35:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011002', 4, 86.00, '2026-08-02T07:36:00'),

    (NEWID(), '00000000-0000-0000-0000-000000011003', 1, 72.00, '2026-08-03T07:30:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011003', 2, 82.00, '2026-08-03T07:35:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011003', 6, 97.00, '2026-08-03T07:36:00'),

    (NEWID(), '00000000-0000-0000-0000-000000011004', 1, 60.00, '2026-08-04T07:30:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011004', 5, 128.00, '2026-08-04T07:35:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011004', 2, 79.00, '2026-08-04T07:36:00'),

    (NEWID(), '00000000-0000-0000-0000-000000011005', 1, 57.00, '2026-08-05T07:30:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011005', 2, 73.00, '2026-08-05T07:35:00'),
    (NEWID(), '00000000-0000-0000-0000-000000011005', 6, 99.00, '2026-08-05T07:36:00');

    -- =====================================================
    -- 7. HEALTH GOALS
    -- =====================================================
    INSERT INTO health_goals
    (
        id, patient_id, metric_type_id, target_value,
        start_date, end_date, status,
        created_by_user_id, created_by_role
    )
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000011001', 1, 65.00, '2026-08-01', '2026-11-01', N'in_progress', '00000000-0000-0000-0000-000000001001', N'patient'),
    (NEWID(), '00000000-0000-0000-0000-000000011002', 3, 120.00, '2026-08-01', '2026-10-01', N'in_progress', '00000000-0000-0000-0000-000000001002', N'patient'),
    (NEWID(), '00000000-0000-0000-0000-000000011003', 2, 75.00, '2026-08-01', '2026-09-30', N'in_progress', '00000000-0000-0000-0000-000000001003', N'patient'),
    (NEWID(), '00000000-0000-0000-0000-000000011004', 5, 95.00, '2026-08-01', '2026-12-31', N'in_progress', '00000000-0000-0000-0000-000000001004', N'patient'),
    (NEWID(), '00000000-0000-0000-0000-000000011005', 1, 55.00, '2026-08-01', '2026-11-30', N'in_progress', '00000000-0000-0000-0000-000000001005', N'patient');

    -- =====================================================
    -- 8. PRODUCTS
    -- =====================================================
    INSERT INTO products
    (
        id, name, description, price, stock_quantity,
        is_prescription_required, active_ingredient,
        therapeutic_group, dosage_form, strength,
        prescription_required
    )
    VALUES
    ('00000000-0000-0000-0000-000000091001',
     N'Paracetamol 500mg (Hộp 10 vỉ x 10 viên)',
     N'Thuốc giảm đau, hạ sốt thông dụng.', 25000, 500, 0,
     N'Paracetamol', N'Giảm đau - Hạ sốt', N'Viên nén', N'500mg', 0),

    ('00000000-0000-0000-0000-000000091002',
     N'Amoxicillin 500mg (Hộp 10 vỉ x 10 viên)',
     N'Kháng sinh nhóm Beta-lactam, chỉ sử dụng theo chỉ định chuyên môn.', 45000, 200, 1,
     N'Amoxicillin', N'Kháng sinh', N'Viên nang', N'500mg', 1),

    ('00000000-0000-0000-0000-000000091003',
     N'Metformin 500mg (Hộp 3 vỉ x 10 viên)',
     N'Sản phẩm dùng trong điều trị đái tháo đường type 2 theo đơn.', 38000, 300, 1,
     N'Metformin', N'Nội tiết - Chuyển hóa', N'Viên nén', N'500mg', 1),

    ('00000000-0000-0000-0000-000000091004',
     N'Losartan 50mg (Hộp 3 vỉ x 10 viên)',
     N'Sản phẩm điều trị tăng huyết áp theo chỉ định bác sĩ.', 52000, 180, 1,
     N'Losartan', N'Tim mạch', N'Viên nén bao phim', N'50mg', 1),

    ('00000000-0000-0000-0000-000000091005',
     N'Vitamin C 500mg (Lọ 100 viên)',
     N'Bổ sung vitamin C và hỗ trợ nhu cầu dinh dưỡng hằng ngày.', 65000, 420, 0,
     N'Ascorbic acid', N'Vitamin - Khoáng chất', N'Viên nén', N'500mg', 0),

    ('00000000-0000-0000-0000-000000091006',
     N'Omeprazole 20mg (Hộp 3 vỉ x 10 viên)',
     N'Sản phẩm hỗ trợ điều trị các bệnh lý liên quan tăng tiết acid dạ dày.', 48000, 150, 1,
     N'Omeprazole', N'Tiêu hóa', N'Viên nang', N'20mg', 1),

    ('00000000-0000-0000-0000-000000091007',
     N'Cetirizine 10mg (Hộp 2 vỉ x 10 viên)',
     N'Sản phẩm kháng histamin dùng trong các trường hợp dị ứng.', 30000, 250, 0,
     N'Cetirizine', N'Kháng dị ứng', N'Viên nén', N'10mg', 0),

    ('00000000-0000-0000-0000-000000091008',
     N'Ventolin xịt định liều 100mcg',
     N'Bình xịt định liều chứa Salbutamol, sử dụng theo chỉ định.', 115000, 80, 1,
     N'Salbutamol', N'Hô hấp', N'Bình xịt', N'100mcg', 1),

    ('00000000-0000-0000-0000-000000091009',
     N'Nước muối sinh lý NaCl 0.9% (Chai 500ml)',
     N'Dung dịch dùng vệ sinh mũi, mắt hoặc làm sạch theo hướng dẫn sử dụng.', 18000, 600, 0,
     N'Sodium Chloride', N'Chăm sóc cá nhân', N'Dung dịch', N'0.9%', 0),

    ('00000000-0000-0000-0000-000000091010',
     N'Khẩu trang y tế 4 lớp (Hộp 50 cái)',
     N'Khẩu trang y tế dùng một lần.', 45000, 1000, 0,
     NULL, N'Vật tư y tế', N'Khác', NULL, 0),

    ('00000000-0000-0000-0000-000000091011',
     N'Ibuprofen 400mg (Hộp 2 vỉ x 10 viên)',
     N'Sản phẩm giảm đau, chống viêm không steroid.', 42000, 220, 0,
     N'Ibuprofen', N'Giảm đau - Kháng viêm', N'Viên nén bao phim', N'400mg', 0),

    ('00000000-0000-0000-0000-000000091012',
     N'Oresol ORS (Hộp 20 gói)',
     N'Bột pha dung dịch bù nước và điện giải.', 36000, 350, 0,
     N'Oral Rehydration Salts', N'Bù nước - Điện giải', N'Bột pha uống', N'1 gói', 0);

    -- =====================================================
    -- 9. MEDICATIONS
    -- Bảng phục vụ prescription; tách biệt products bán hàng.
    -- =====================================================
    INSERT INTO medications (id, name, generic_name, category)
    VALUES
    ('00000000-0000-0000-0000-000000071001', N'Paracetamol 500mg', N'Paracetamol', N'Giảm đau - Hạ sốt'),
    ('00000000-0000-0000-0000-000000071002', N'Amoxicillin 500mg', N'Amoxicillin', N'Kháng sinh'),
    ('00000000-0000-0000-0000-000000071003', N'Metformin 500mg', N'Metformin', N'Nội tiết - Chuyển hóa'),
    ('00000000-0000-0000-0000-000000071004', N'Losartan 50mg', N'Losartan', N'Tim mạch'),
    ('00000000-0000-0000-0000-000000071005', N'Vitamin C 500mg', N'Ascorbic acid', N'Vitamin - Khoáng chất'),
    ('00000000-0000-0000-0000-000000071006', N'Omeprazole 20mg', N'Omeprazole', N'Tiêu hóa'),
    ('00000000-0000-0000-0000-000000071007', N'Cetirizine 10mg', N'Cetirizine', N'Kháng dị ứng'),
    ('00000000-0000-0000-0000-000000071008', N'Salbutamol 100mcg', N'Salbutamol', N'Hô hấp'),
    ('00000000-0000-0000-0000-000000071009', N'Ibuprofen 400mg', N'Ibuprofen', N'Giảm đau - Kháng viêm'),
    ('00000000-0000-0000-0000-000000071010', N'Oresol ORS', N'Oral Rehydration Salts', N'Bù nước - Điện giải');

    -- Đồng bộ Product -> Medication để nghiệp vụ cấp phát thuốc kiểm tra đúng MedicationId.
    UPDATE products
    SET medication_id = CASE id
        WHEN '00000000-0000-0000-0000-000000091001' THEN '00000000-0000-0000-0000-000000071001'
        WHEN '00000000-0000-0000-0000-000000091002' THEN '00000000-0000-0000-0000-000000071002'
        WHEN '00000000-0000-0000-0000-000000091003' THEN '00000000-0000-0000-0000-000000071003'
        WHEN '00000000-0000-0000-0000-000000091004' THEN '00000000-0000-0000-0000-000000071004'
        WHEN '00000000-0000-0000-0000-000000091005' THEN '00000000-0000-0000-0000-000000071005'
        WHEN '00000000-0000-0000-0000-000000091006' THEN '00000000-0000-0000-0000-000000071006'
        WHEN '00000000-0000-0000-0000-000000091007' THEN '00000000-0000-0000-0000-000000071007'
        WHEN '00000000-0000-0000-0000-000000091008' THEN '00000000-0000-0000-0000-000000071008'
        WHEN '00000000-0000-0000-0000-000000091011' THEN '00000000-0000-0000-0000-000000071009'
        WHEN '00000000-0000-0000-0000-000000091012' THEN '00000000-0000-0000-0000-000000071010'
    END
    WHERE id IN (
        '00000000-0000-0000-0000-000000091001',
        '00000000-0000-0000-0000-000000091002',
        '00000000-0000-0000-0000-000000091003',
        '00000000-0000-0000-0000-000000091004',
        '00000000-0000-0000-0000-000000091005',
        '00000000-0000-0000-0000-000000091006',
        '00000000-0000-0000-0000-000000091007',
        '00000000-0000-0000-0000-000000091008',
        '00000000-0000-0000-0000-000000091011',
        '00000000-0000-0000-0000-000000091012'
    );

    -- =====================================================
    -- 10. SYMPTOM LOGS
    -- Dữ liệu mẫu cho màn hình lịch sử phân tích AI sau này.
    -- =====================================================
    INSERT INTO symptom_logs
    (id, patient_id, symptoms_description, predicted_disease, severity_level, ai_advice, created_at)
    VALUES
    ('00000000-0000-0000-0000-000000131001',
     '00000000-0000-0000-0000-000000011003',
     N'Ho khan, sổ mũi, đau họng nhẹ trong 2 ngày.',
     N'Cảm cúm thông thường', N'LOW',
     N'Nghỉ ngơi, uống đủ nước và theo dõi triệu chứng. Nếu diễn tiến nặng cần đi khám.',
     '2026-07-08T20:00:00'),

    ('00000000-0000-0000-0000-000000131002',
     '00000000-0000-0000-0000-000000011005',
     N'Đau bụng dữ dội vùng hạ sườn phải, kèm buồn nôn.',
     N'Nghi tình trạng cần đánh giá cấp cứu', N'HIGH',
     N'Nên đến cơ sở y tế ngay để được bác sĩ thăm khám trực tiếp.',
     '2026-07-11T22:15:00'),

    ('00000000-0000-0000-0000-000000131003',
     '00000000-0000-0000-0000-000000011001',
     N'Ngứa da, nổi mẩn đỏ sau khi ăn hải sản.',
     N'Dị ứng thực phẩm', N'LOW',
     N'Tránh tiếp xúc tác nhân nghi ngờ và theo dõi triệu chứng; đi khám nếu khó thở hoặc sưng phù.',
     '2026-07-12T18:30:00'),

    ('00000000-0000-0000-0000-000000131004',
     '00000000-0000-0000-0000-000000011004',
     N'Ợ nóng, đau vùng thượng vị sau ăn, tái diễn nhiều ngày.',
     N'Viêm loét dạ dày nhẹ', N'LOW',
     N'Nên điều chỉnh chế độ ăn và đặt lịch bác sĩ nếu triệu chứng kéo dài.',
     '2026-08-02T21:00:00');

    -- =====================================================
    -- 11. AUTO PRESCRIPTIONS
    -- Chỉ là mapping dữ liệu demo của hệ thống.
    -- =====================================================
    INSERT INTO auto_prescriptions
    (id, disease_name, recommended_product_id, dosage_instructions)
    VALUES
    ('00000000-0000-0000-0000-000000141001', N'Cảm cúm thông thường',
     '00000000-0000-0000-0000-000000091001',
     N'Dữ liệu demo: sử dụng sản phẩm theo hướng dẫn chuyên môn và hướng dẫn trên nhãn.'),

    ('00000000-0000-0000-0000-000000141002', N'Dị ứng thực phẩm',
     '00000000-0000-0000-0000-000000091007',
     N'Dữ liệu demo: chỉ gợi ý sản phẩm; cần kiểm tra chống chỉ định và triệu chứng cảnh báo.'),

    ('00000000-0000-0000-0000-000000141003', N'Viêm loét dạ dày nhẹ',
     '00000000-0000-0000-0000-000000091006',
     N'Dữ liệu demo: việc dùng thuốc kê đơn cần được bác sĩ/dược sĩ xác nhận.'),

    ('00000000-0000-0000-0000-000000141004', N'Đau đầu nhẹ',
     '00000000-0000-0000-0000-000000091011',
     N'Dữ liệu demo: chỉ dùng để thử chức năng AutoPrescription.');

    -- =====================================================
    -- 12. CHAT SESSIONS
    -- =====================================================
    INSERT INTO chat_sessions
    (id, patient_id, doctor_id, ai_symptom_log_id, status, created_at)
    VALUES
    ('00000000-0000-0000-0000-000000041001',
     '00000000-0000-0000-0000-000000011001',
     '00000000-0000-0000-0000-000000031001',
     '00000000-0000-0000-0000-000000131003',
     N'active', '2026-08-10T08:30:00'),

    ('00000000-0000-0000-0000-000000041002',
     '00000000-0000-0000-0000-000000011002',
     '00000000-0000-0000-0000-000000031003',
     NULL, N'closed', '2026-08-05T14:00:00'),

    ('00000000-0000-0000-0000-000000041003',
     '00000000-0000-0000-0000-000000011003',
     '00000000-0000-0000-0000-000000031002',
     '00000000-0000-0000-0000-000000131001',
     N'active', '2026-08-12T19:00:00'),

    ('00000000-0000-0000-0000-000000041004',
     '00000000-0000-0000-0000-000000011004',
     '00000000-0000-0000-0000-000000031003',
     '00000000-0000-0000-0000-000000131004',
     N'active', '2026-08-13T10:15:00');

    -- =====================================================
    -- 13. CHAT MESSAGES
    -- =====================================================
    INSERT INTO chat_messages
    (id, session_id, sender_id, message, is_read, sent_at)
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000041001', '00000000-0000-0000-0000-000000001001',
     N'Chào bác sĩ, gần đây tôi đôi lúc thấy tim đập nhanh khi vận động.', 1, '2026-08-10T08:31:00'),
    (NEWID(), '00000000-0000-0000-0000-000000041001', '00000000-0000-0000-0000-000000002001',
     N'Bạn có kèm đau ngực, khó thở hoặc chóng mặt không?', 1, '2026-08-10T08:33:00'),
    (NEWID(), '00000000-0000-0000-0000-000000041001', '00000000-0000-0000-0000-000000001001',
     N'Thỉnh thoảng hơi mệt nhưng chưa thấy đau ngực.', 0, '2026-08-10T08:35:00'),

    (NEWID(), '00000000-0000-0000-0000-000000041002', '00000000-0000-0000-0000-000000001002',
     N'Tôi muốn hỏi cách theo dõi huyết áp tại nhà.', 1, '2026-08-05T14:01:00'),
    (NEWID(), '00000000-0000-0000-0000-000000041002', '00000000-0000-0000-0000-000000002003',
     N'Bạn nên đo vào thời điểm cố định và ghi lại kết quả để bác sĩ đánh giá xu hướng.', 1, '2026-08-05T14:05:00'),

    (NEWID(), '00000000-0000-0000-0000-000000041003', '00000000-0000-0000-0000-000000001003',
     N'Tôi bị ho và sổ mũi vài ngày, muốn được tư vấn.', 1, '2026-08-12T19:01:00'),
    (NEWID(), '00000000-0000-0000-0000-000000041003', '00000000-0000-0000-0000-000000002002',
     N'Bạn có sốt cao hoặc khó thở không?', 0, '2026-08-12T19:04:00'),

    (NEWID(), '00000000-0000-0000-0000-000000041004', '00000000-0000-0000-0000-000000001004',
     N'Tôi hay đau vùng thượng vị sau bữa ăn.', 1, '2026-08-13T10:16:00'),
    (NEWID(), '00000000-0000-0000-0000-000000041004', '00000000-0000-0000-0000-000000002003',
     N'Tôi sẽ hỏi thêm về thời gian đau và các thuốc bạn đang dùng.', 0, '2026-08-13T10:18:00');

    -- =====================================================
    -- 14. APPOINTMENTS
    -- =====================================================
    INSERT INTO appointments
    (id, chat_session_id, patient_id, doctor_id, scheduled_at, type, status, created_at)
    VALUES
    ('00000000-0000-0000-0000-000000051001',
     '00000000-0000-0000-0000-000000041001',
     '00000000-0000-0000-0000-000000011001',
     '00000000-0000-0000-0000-000000031001',
     '2026-08-22T09:00:00', N'online_consult', N'confirmed', '2026-08-10T08:40:00'),

    ('00000000-0000-0000-0000-000000051002',
     '00000000-0000-0000-0000-000000041002',
     '00000000-0000-0000-0000-000000011002',
     '00000000-0000-0000-0000-000000031003',
     '2026-08-06T14:30:00', N'clinic_visit', N'completed', '2026-08-05T14:10:00'),

    ('00000000-0000-0000-0000-000000051003',
     '00000000-0000-0000-0000-000000041003',
     '00000000-0000-0000-0000-000000011003',
     '00000000-0000-0000-0000-000000031002',
     '2026-08-23T10:00:00', N'online_consult', N'pending', '2026-08-12T19:10:00'),

    ('00000000-0000-0000-0000-000000051004',
     '00000000-0000-0000-0000-000000041004',
     '00000000-0000-0000-0000-000000011004',
     '00000000-0000-0000-0000-000000031003',
     '2026-08-14T10:30:00', N'clinic_visit', N'completed', '2026-08-13T10:25:00'),

    ('00000000-0000-0000-0000-000000051005',
     NULL,
     '00000000-0000-0000-0000-000000011001',
     '00000000-0000-0000-0000-000000031003',
     '2026-08-28T15:00:00', N'clinic_visit', N'pending', '2026-08-20T09:00:00');

    -- =====================================================
    -- 15. MEDICAL RECORDS
    -- =====================================================
    INSERT INTO medical_records
    (id, appointment_id, patient_id, doctor_id, diagnosis, notes, created_at)
    VALUES
    ('00000000-0000-0000-0000-000000061001',
     '00000000-0000-0000-0000-000000051002',
     '00000000-0000-0000-0000-000000011002',
     '00000000-0000-0000-0000-000000031003',
     N'Tăng huyết áp cần tiếp tục theo dõi',
     N'Khuyến nghị ghi nhận huyết áp tại nhà và tái khám theo lịch.',
     '2026-08-06T15:10:00'),

    ('00000000-0000-0000-0000-000000061002',
     '00000000-0000-0000-0000-000000051004',
     '00000000-0000-0000-0000-000000011004',
     '00000000-0000-0000-0000-000000031003',
     N'Rối loạn tiêu hóa, theo dõi triệu chứng dạ dày',
     N'Điều chỉnh chế độ ăn và tái khám nếu triệu chứng kéo dài.',
     '2026-08-14T11:10:00');

    -- =====================================================
    -- 16. PRESCRIPTIONS
    -- =====================================================
    INSERT INTO prescriptions
    (
        id, appointment_id, medical_record_id, patient_id,
        doctor_id, diagnosis, status, created_at
    )
    VALUES
    ('00000000-0000-0000-0000-000000081001',
     '00000000-0000-0000-0000-000000051002',
     '00000000-0000-0000-0000-000000061001',
     '00000000-0000-0000-0000-000000011002',
     '00000000-0000-0000-0000-000000031003',
     N'Tăng huyết áp cần tiếp tục theo dõi',
     N'pending', '2026-08-06T15:15:00'),

    ('00000000-0000-0000-0000-000000081002',
     '00000000-0000-0000-0000-000000051004',
     '00000000-0000-0000-0000-000000061002',
     '00000000-0000-0000-0000-000000011004',
     '00000000-0000-0000-0000-000000031003',
     N'Rối loạn tiêu hóa, theo dõi triệu chứng dạ dày',
     N'approved', '2026-08-14T11:15:00');

    -- =====================================================
    -- 17. PRESCRIPTION ITEMS
    -- =====================================================
    INSERT INTO prescription_items
    (id, prescription_id, medication_id, dosage, frequency_per_day, duration_days, instructions)
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000081001', '00000000-0000-0000-0000-000000071004',
     N'50mg', 1, 30, N'Uống theo hướng dẫn của bác sĩ.'),

    (NEWID(), '00000000-0000-0000-0000-000000081001', '00000000-0000-0000-0000-000000071005',
     N'500mg', 1, 14, N'Dùng theo chỉ định trong đơn.'),

    (NEWID(), '00000000-0000-0000-0000-000000081002', '00000000-0000-0000-0000-000000071006',
     N'20mg', 1, 14, N'Dùng theo chỉ định bác sĩ và hướng dẫn sử dụng.');

    -- =====================================================
    -- 18. PAYMENTS
    -- =====================================================
    INSERT INTO payments
    (id, appointment_id, patient_id, amount, platform_fee, status, payment_method, transaction_ref, created_at)
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000051001', '00000000-0000-0000-0000-000000011001',
     300000, 15000, N'paid', N'momo', N'TXN-20260810-0001', '2026-08-10T08:45:00'),

    (NEWID(), '00000000-0000-0000-0000-000000051002', '00000000-0000-0000-0000-000000011002',
     280000, 14000, N'paid', N'vnpay', N'TXN-20260805-0002', '2026-08-05T14:12:00'),

    (NEWID(), '00000000-0000-0000-0000-000000051003', '00000000-0000-0000-0000-000000011003',
     250000, 12500, N'pending', N'bank_transfer', N'TXN-20260812-0003', '2026-08-12T19:12:00'),

    (NEWID(), '00000000-0000-0000-0000-000000051004', '00000000-0000-0000-0000-000000011004',
     280000, 14000, N'paid', N'cash', N'TXN-20260813-0004', '2026-08-13T10:30:00');

    -- =====================================================
    -- 19. NOTIFICATIONS
    -- =====================================================
    INSERT INTO notifications
    (id, user_id, type, title, message, is_read, created_at)
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000001001', N'appointment_reminder',
     N'Nhắc lịch khám', N'Bạn có lịch khám trực tuyến vào 09:00 ngày 22/08/2026.', 0, '2026-08-21T08:00:00'),

    (NEWID(), '00000000-0000-0000-0000-000000001002', N'health_metric',
     N'Theo dõi huyết áp', N'Bạn nên tiếp tục cập nhật huyết áp hằng ngày.', 0, '2026-08-07T08:00:00'),

    (NEWID(), '00000000-0000-0000-0000-000000001003', N'appointment_confirmed',
     N'Lịch khám đã được ghi nhận', N'Lịch tư vấn của bạn đang chờ bác sĩ xác nhận.', 1, '2026-08-12T19:15:00'),

    (NEWID(), '00000000-0000-0000-0000-000000001004', N'prescription_ready',
     N'Đơn thuốc mới', N'Đơn thuốc của bạn đã được cập nhật trong hồ sơ.', 0, '2026-08-14T11:20:00'),

    (NEWID(), '00000000-0000-0000-0000-000000002001', N'new_appointment',
     N'Lịch tư vấn mới', N'Bạn có một lịch tư vấn với bệnh nhân Nguyễn Văn An.', 0, '2026-08-20T09:30:00'),

    (NEWID(), '00000000-0000-0000-0000-000000004001', N'inventory_low_stock',
     N'Cảnh báo tồn kho', N'Sản phẩm Ventolin có lượng tồn kho thấp.', 0, '2026-08-20T07:00:00'),

    (NEWID(), '00000000-0000-0000-0000-000000003001', N'inventory_approval',
     N'Phiếu nhập chờ duyệt', N'Có phiếu nhập kho mới đang chờ Admin duyệt.', 0, '2026-08-20T10:00:00');

    -- =====================================================
    -- 20. DRUG ALTERNATIVES
    -- =====================================================
    INSERT INTO drug_alternatives
    (id, product_id, alternative_product_id, reason, similarity_score, created_at)
    VALUES
    (NEWID(),
     '00000000-0000-0000-0000-000000091001',
     '00000000-0000-0000-0000-000000091011',
     N'Cùng nhóm giảm đau nhưng khác hoạt chất; cần kiểm tra tính phù hợp.',
     70.00, '2026-08-01T00:00:00'),

    (NEWID(),
     '00000000-0000-0000-0000-000000091011',
     '00000000-0000-0000-0000-000000091001',
     N'Phương án thay thế trong nhóm giảm đau khi phù hợp với người dùng.',
     70.00, '2026-08-01T00:00:00'),

    (NEWID(),
     '00000000-0000-0000-0000-000000091006',
     '00000000-0000-0000-0000-000000091009',
     N'Dữ liệu demo cho chức năng DrugAlternative; không đồng nghĩa tương đương điều trị.',
     20.00, '2026-08-01T00:00:00');

    -- =====================================================
    -- 21. ECOMMERCE ORDERS
    -- =====================================================
    INSERT INTO ecommerce_orders
    (
        id, user_id, guest_phone, shipping_address,
        total_amount, payment_method, payment_status,
        order_status, prescription_id, created_at
    )
    VALUES
    ('00000000-0000-0000-0000-000000101001',
     '00000000-0000-0000-0000-000000001001', NULL,
     N'12 Lê Lợi, Quận 1, TP.HCM',
     83000, N'cod', N'paid', N'delivered', NULL, '2026-08-01T10:00:00'),

    ('00000000-0000-0000-0000-000000101002',
     '00000000-0000-0000-0000-000000001004', NULL,
     N'78 Cách Mạng Tháng 8, Quận 3, TP.HCM',
     100000, N'cod', N'pending', N'processing',
     '00000000-0000-0000-0000-000000081002', '2026-08-14T12:00:00'),

    ('00000000-0000-0000-0000-000000101003',
     NULL, N'0909998877',
     N'156 Trần Hưng Đạo, Quận 5, TP.HCM',
     63000, N'cod', N'pending', N'processing',
     NULL, '2026-08-18T09:00:00'),

    ('00000000-0000-0000-0000-000000101004',
     '00000000-0000-0000-0000-000000001003', NULL,
     N'32 Nguyễn Thị Minh Khai, Quận 1, TP.HCM',
     67000, N'cod', N'pending', N'processing',
     NULL, '2026-08-20T16:00:00');

    -- =====================================================
    -- 22. ORDER ITEMS
    -- Tổng tiền khớp ecommerce_orders.
    -- =====================================================
    INSERT INTO order_items
    (id, order_id, product_id, quantity, unit_price)
    VALUES
    -- Order 101001 = 65,000 + 18,000 = 83,000
    (NEWID(), '00000000-0000-0000-0000-000000101001',
     '00000000-0000-0000-0000-000000091005', 1, 65000),
    (NEWID(), '00000000-0000-0000-0000-000000101001',
     '00000000-0000-0000-0000-000000091009', 1, 18000),

    -- Order 101002 = 48,000 + 52,000 = 100,000
    (NEWID(), '00000000-0000-0000-0000-000000101002',
     '00000000-0000-0000-0000-000000091006', 1, 48000),
    (NEWID(), '00000000-0000-0000-0000-000000101002',
     '00000000-0000-0000-0000-000000091004', 1, 52000),

    -- Guest order = 45,000 + 18,000 = 63,000
    (NEWID(), '00000000-0000-0000-0000-000000101003',
     '00000000-0000-0000-0000-000000091010', 1, 45000),
    (NEWID(), '00000000-0000-0000-0000-000000101003',
     '00000000-0000-0000-0000-000000091009', 1, 18000),

    -- Order 101004 = 25,000 + 42,000 = 67,000
    (NEWID(), '00000000-0000-0000-0000-000000101004',
     '00000000-0000-0000-0000-000000091001', 1, 25000),
    (NEWID(), '00000000-0000-0000-0000-000000101004',
     '00000000-0000-0000-0000-000000091011', 1, 42000);

    -- =====================================================
    -- 23. SUPPLIERS
    -- =====================================================
    INSERT INTO suppliers
    (id, supplier_name, contact_phone, gdp_certified, created_at)
    VALUES
    ('00000000-0000-0000-0000-000000111001',
     N'Công ty CP Dược phẩm Trung ương 2', N'02836123456', 1, '2025-08-01T08:00:00'),

    ('00000000-0000-0000-0000-000000111002',
     N'Công ty TNHH Zuellig Pharma Việt Nam', N'02839876543', 1, '2025-08-02T08:00:00'),

    ('00000000-0000-0000-0000-000000111003',
     N'Công ty CP Dược Hậu Giang', N'02923891433', 1, '2025-08-03T08:00:00');

    -- =====================================================
    -- 24. INVENTORY RECEIPTS
    -- =====================================================
    INSERT INTO inventory_receipts
    (
        id, supplier_id, created_by, approved_by,
        total_cost, receipt_date, status
    )
    VALUES
    ('00000000-0000-0000-0000-000000121001',
     '00000000-0000-0000-0000-000000111001',
     '00000000-0000-0000-0000-000000004001',
     '00000000-0000-0000-0000-000000003001',
     12500000, '2026-05-20T09:00:00', N'approved'),

    ('00000000-0000-0000-0000-000000121002',
     '00000000-0000-0000-0000-000000111002',
     '00000000-0000-0000-0000-000000004002',
     '00000000-0000-0000-0000-000000003001',
     8200000, '2026-06-10T09:00:00', N'approved'),

    ('00000000-0000-0000-0000-000000121003',
     '00000000-0000-0000-0000-000000111003',
     '00000000-0000-0000-0000-000000004001',
     NULL,
     3400000, '2026-08-20T09:00:00', N'pending_approval');

    -- =====================================================
    -- 25. RECEIPT DETAILS
    -- Dữ liệu lô hàng phục vụ FEFO.
    -- =====================================================
    INSERT INTO receipt_details
    (id, receipt_id, product_id, batch_number, expiration_date, quantity_imported, import_price)
    VALUES
    (NEWID(), '00000000-0000-0000-0000-000000121001',
     '00000000-0000-0000-0000-000000091001',
     N'PCT2026A01', '2028-05-01', 5000, 1500),

    (NEWID(), '00000000-0000-0000-0000-000000121001',
     '00000000-0000-0000-0000-000000091002',
     N'AMX2026A02', '2027-11-01', 2000, 22000),

    (NEWID(), '00000000-0000-0000-0000-000000121002',
     '00000000-0000-0000-0000-000000091004',
     N'LOS2026B01', '2028-01-15', 1000, 32000),

    (NEWID(), '00000000-0000-0000-0000-000000121002',
     '00000000-0000-0000-0000-000000091008',
     N'VEN2026B02', '2027-09-20', 300, 75000),

    (NEWID(), '00000000-0000-0000-0000-000000121003',
     '00000000-0000-0000-0000-000000091005',
     N'VTC2026C01', '2027-12-01', 3000, 40000),

    (NEWID(), '00000000-0000-0000-0000-000000121003',
     '00000000-0000-0000-0000-000000091011',
     N'IBU2026C02', '2027-10-15', 1500, 24000);

    COMMIT TRANSACTION;

    PRINT N'=========================================================';
    PRINT N'BLUE_CROWN seed data hoàn tất.';
    PRINT N'Mật khẩu chung cho tài khoản test: BlueCrown@123';
    PRINT N'=========================================================';

    -- =====================================================
    -- 26. KIỂM TRA NHANH
    -- =====================================================
    SELECT N'users' AS table_name, COUNT(*) AS row_count FROM users
    UNION ALL SELECT N'patient_profiles', COUNT(*) FROM patient_profiles
    UNION ALL SELECT N'doctor_profiles', COUNT(*) FROM doctor_profiles
    UNION ALL SELECT N'products', COUNT(*) FROM products
    UNION ALL SELECT N'medications', COUNT(*) FROM medications
    UNION ALL SELECT N'appointments', COUNT(*) FROM appointments
    UNION ALL SELECT N'prescriptions', COUNT(*) FROM prescriptions
    UNION ALL SELECT N'ecommerce_orders', COUNT(*) FROM ecommerce_orders
    UNION ALL SELECT N'inventory_receipts', COUNT(*) FROM inventory_receipts
    UNION ALL SELECT N'symptom_logs', COUNT(*) FROM symptom_logs;

    SELECT
        id,
        name,
        price,
        stock_quantity,
        active_ingredient,
        therapeutic_group,
        is_prescription_required
    FROM products
    ORDER BY name;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

-- Bổ sung dữ liệu
USE BLUE_CROWN;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
-- BLUE CROWN - ADD MORE DEMO DATA
-- Chạy sau BLUE_CROWN_RESET_AND_SEED.sql. Không xóa dữ liệu hiện có.
BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @PasswordHash NVARCHAR(255) = N'$2y$12$8XHxIMNCvtGl2Tw8yWgmkebH5.9/aVPyf9fQflXVVEcqUwAMvjZ5W';

    -- 1. USERS
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001006') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001006',N'gia.hoang@gmail.com',N'0901001006',@PasswordHash,N'Hoàng Minh Gia','1997-04-14',N'male',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001007') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001007',N'han.nguyen@gmail.com',N'0901001007',@PasswordHash,N'Nguyễn Thu Hân','2001-08-09',N'female',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001008') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001008',N'khang.vo@gmail.com',N'0901001008',@PasswordHash,N'Võ Quốc Khang','1993-01-27',N'male',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001009') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001009',N'linh.truong@gmail.com',N'0901001009',@PasswordHash,N'Trương Mỹ Linh','1999-12-11',N'female',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001010') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001010',N'minh.pham@gmail.com',N'0901001010',@PasswordHash,N'Phạm Hoàng Minh','1986-06-30',N'male',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001011') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001011',N'nhu.le@gmail.com',N'0901001011',@PasswordHash,N'Lê Quỳnh Như','2003-03-18',N'female',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001012') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001012',N'phong.bui@gmail.com',N'0901001012',@PasswordHash,N'Bùi Thanh Phong','1991-10-05',N'male',N'patient',N'suspended','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001013') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001013',N'quyen.dao@gmail.com',N'0901001013',@PasswordHash,N'Đào Ngọc Quyên','1996-01-22',N'female',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001014') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001014',N'son.ngo@gmail.com',N'0901001014',@PasswordHash,N'Ngô Minh Sơn','1989-09-17',N'male',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000001015') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000001015',N'thao.tran@gmail.com',N'0901001015',@PasswordHash,N'Trần Phương Thảo','2000-11-30',N'female',N'patient',N'active','2026-03-01','2026-02-25','2026-03-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000002004') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000002004',N'mai.tran@bluecrown.vn',N'0912002004',@PasswordHash,N'BS. Trần Thanh Mai','1985-05-09',N'female',N'doctor',N'active','2025-12-01','2025-11-20','2025-12-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000002005') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000002005',N'nam.nguyen@bluecrown.vn',N'0912002005',@PasswordHash,N'BS. Nguyễn Đức Nam','1981-09-16',N'male',N'doctor',N'active','2025-12-01','2025-11-20','2025-12-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000002006') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000002006',N'thy.le@bluecrown.vn',N'0912002006',@PasswordHash,N'BS. Lê Ngọc Thy','1987-02-21',N'female',N'doctor',N'active','2025-12-01','2025-11-20','2025-12-01');
    IF NOT EXISTS (SELECT 1 FROM users WHERE id='00000000-0000-0000-0000-000000004003') INSERT INTO users (id,email,phone,password_hash,full_name,date_of_birth,gender,role,status,email_verified_at,created_at,updated_at) VALUES ('00000000-0000-0000-0000-000000004003',N'minh.do@bluecrown.vn',N'0934004003',@PasswordHash,N'DS. Đỗ Quang Minh','1995-07-12',N'male',N'pharmacist',N'active','2025-10-01','2025-09-25','2025-10-01');

    -- 2. PATIENT PROFILES
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011006') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011006','00000000-0000-0000-0000-000000001006',N'A-',170,66,N'Không',N'Viêm mũi dị ứng',N'Người liên hệ 6',N'0987001006');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011007') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011007','00000000-0000-0000-0000-000000001007',N'O+',158,50,N'Dị ứng phấn hoa',N'Không',N'Người liên hệ 7',N'0987001007');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011008') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011008','00000000-0000-0000-0000-000000001008',N'B+',178,80,N'Không',N'Mỡ máu cao',N'Người liên hệ 8',N'0987001008');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011009') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011009','00000000-0000-0000-0000-000000001009',N'AB+',162,55,N'Dị ứng bụi',N'Không',N'Người liên hệ 9',N'0987001009');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011010') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011010','00000000-0000-0000-0000-000000001010',N'O-',174,76,N'Không',N'Tăng huyết áp',N'Người liên hệ 10',N'0987001010');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011011') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011011','00000000-0000-0000-0000-000000001011',N'A+',160,52,N'Không',N'Không',N'Người liên hệ 11',N'0987001011');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011012') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011012','00000000-0000-0000-0000-000000001012',N'B-',176,73,N'Dị ứng Aspirin',N'Viêm dạ dày',N'Người liên hệ 12',N'0987001012');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011013') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011013','00000000-0000-0000-0000-000000001013',N'O+',168,61,N'Không',N'Không',N'Người liên hệ 13',N'0987001013');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011014') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011014','00000000-0000-0000-0000-000000001014',N'A+',172,70,N'Dị ứng hải sản',N'Không',N'Người liên hệ 14',N'0987001014');
    IF NOT EXISTS (SELECT 1 FROM patient_profiles WHERE id='00000000-0000-0000-0000-000000011015') INSERT INTO patient_profiles (id,user_id,blood_type,height_cm,WeightKg,allergies,chronic_conditions,emergency_contact_name,emergency_contact_phone) VALUES ('00000000-0000-0000-0000-000000011015','00000000-0000-0000-0000-000000001015',N'AB-',159,54,N'Không',N'Hen nhẹ',N'Người liên hệ 15',N'0987001015');

    -- 3. CLINICS + DOCTOR PROFILES
    IF NOT EXISTS (SELECT 1 FROM clinics WHERE id='00000000-0000-0000-0000-000000021004') INSERT INTO clinics(id,name,address,phone) VALUES('00000000-0000-0000-0000-000000021004',N'Phòng khám Da liễu Blue Crown Phú Nhuận',N'120 Phan Xích Long, Quận Phú Nhuận, TP.HCM',N'02838221004');
    IF NOT EXISTS (SELECT 1 FROM clinics WHERE id='00000000-0000-0000-0000-000000021005') INSERT INTO clinics(id,name,address,phone) VALUES('00000000-0000-0000-0000-000000021005',N'Phòng khám Hô hấp Blue Crown Tân Bình',N'215 Cộng Hòa, Quận Tân Bình, TP.HCM',N'02838221005');
    IF NOT EXISTS (SELECT 1 FROM clinics WHERE id='00000000-0000-0000-0000-000000021006') INSERT INTO clinics(id,name,address,phone) VALUES('00000000-0000-0000-0000-000000021006',N'Phòng khám Tiêu hóa Blue Crown Quận 10',N'80 Thành Thái, Quận 10, TP.HCM',N'02838221006');
    IF NOT EXISTS (SELECT 1 FROM doctor_profiles WHERE id='00000000-0000-0000-0000-000000031004') INSERT INTO doctor_profiles(id,user_id,specialty,license_number,license_verified,bio,years_experience,clinic_id,consultation_fee,rating_avg,rating_count) VALUES('00000000-0000-0000-0000-000000031004','00000000-0000-0000-0000-000000002004',N'Da liễu',N'VN-DER-2018-0204',1,N'Bác sĩ chuyên khoa da liễu tại Blue Crown.',10,'00000000-0000-0000-0000-000000021004',270000,4.85,188);
    IF NOT EXISTS (SELECT 1 FROM doctor_profiles WHERE id='00000000-0000-0000-0000-000000031005') INSERT INTO doctor_profiles(id,user_id,specialty,license_number,license_verified,bio,years_experience,clinic_id,consultation_fee,rating_avg,rating_count) VALUES('00000000-0000-0000-0000-000000031005','00000000-0000-0000-0000-000000002005',N'Hô hấp',N'VN-RES-2013-0119',1,N'Bác sĩ chuyên khoa hô hấp tại Blue Crown.',16,'00000000-0000-0000-0000-000000021005',320000,4.78,145);
    IF NOT EXISTS (SELECT 1 FROM doctor_profiles WHERE id='00000000-0000-0000-0000-000000031006') INSERT INTO doctor_profiles(id,user_id,specialty,license_number,license_verified,bio,years_experience,clinic_id,consultation_fee,rating_avg,rating_count) VALUES('00000000-0000-0000-0000-000000031006','00000000-0000-0000-0000-000000002006',N'Tiêu hóa',N'VN-GAS-2016-0311',1,N'Bác sĩ chuyên khoa tiêu hóa tại Blue Crown.',13,'00000000-0000-0000-0000-000000021006',300000,4.92,231);

    -- 4. PRODUCTS (thêm 20, tổng khoảng 32)
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091013') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091013',N'Aspirin 81mg (Hộp 3 vỉ x 10 viên)',N'Sản phẩm chứa aspirin liều thấp, sử dụng theo hướng dẫn chuyên môn.',39000,180,1,N'Acetylsalicylic acid',N'Tim mạch',N'Viên nén',N'81mg',1);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091014') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091014',N'Diclofenac Gel 1% (Tuýp 20g)',N'Gel bôi ngoài da thuộc nhóm kháng viêm không steroid.',52000,210,0,N'Diclofenac sodium',N'Giảm đau - Kháng viêm',N'Gel bôi ngoài da',N'1%',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091015') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091015',N'Loratadine 10mg (Hộp 10 viên)',N'Sản phẩm kháng histamin dùng trong các triệu chứng dị ứng.',28000,320,0,N'Loratadine',N'Kháng dị ứng',N'Viên nén',N'10mg',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091016') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091016',N'Chlorpheniramine 4mg (Hộp 100 viên)',N'Sản phẩm kháng histamin thế hệ cũ.',22000,400,0,N'Chlorpheniramine maleate',N'Kháng dị ứng',N'Viên nén',N'4mg',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091017') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091017',N'Kẽm Zinc 10mg (Hộp 30 viên)',N'Bổ sung kẽm cho nhu cầu dinh dưỡng hằng ngày.',78000,250,0,N'Zinc gluconate',N'Vitamin - Khoáng chất',N'Viên nén',N'10mg',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091018') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091018',N'Calcium + Vitamin D3 (Hộp 60 viên)',N'Bổ sung canxi và vitamin D3.',145000,170,0,N'Calcium carbonate + Cholecalciferol',N'Vitamin - Khoáng chất',N'Viên nén',N'600mg + 400IU',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091019') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091019',N'Povidone Iodine 10% (Chai 90ml)',N'Dung dịch sát khuẩn dùng ngoài da.',38000,450,0,N'Povidone iodine',N'Sát khuẩn',N'Dung dịch dùng ngoài',N'10%',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091020') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091020',N'Cồn y tế 70 độ (Chai 500ml)',N'Dung dịch cồn dùng cho mục đích sát khuẩn ngoài da.',32000,700,0,N'Ethanol',N'Sát khuẩn',N'Dung dịch',N'70%',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091021') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091021',N'Gel rửa tay khô 500ml',N'Gel vệ sinh tay nhanh dùng ngoài da.',69000,520,0,N'Ethanol',N'Chăm sóc cá nhân',N'Gel',N'500ml',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091022') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091022',N'Nhiệt kế điện tử',N'Thiết bị đo thân nhiệt điện tử dùng tại nhà.',89000,160,0,NULL,N'Thiết bị y tế',N'Thiết bị',NULL,0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091023') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091023',N'Dextromethorphan Syrup 15mg/5ml (Chai 100ml)',N'Siro chứa dextromethorphan dùng trong các trường hợp ho theo hướng dẫn.',57000,190,0,N'Dextromethorphan',N'Hô hấp',N'Siro',N'15mg/5ml',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091024') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091024',N'Guaifenesin Syrup 100mg/5ml (Chai 100ml)',N'Siro chứa guaifenesin hỗ trợ long đờm.',61000,175,0,N'Guaifenesin',N'Hô hấp',N'Siro',N'100mg/5ml',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091025') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091025',N'Xịt mũi nước muối biển 70ml',N'Dung dịch vệ sinh và làm ẩm niêm mạc mũi.',72000,300,0,N'Sodium Chloride',N'Tai mũi họng',N'Dung dịch xịt',N'70ml',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091026') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091026',N'Nước mắt nhân tạo 0.5% (Lọ 15ml)',N'Dung dịch nhỏ mắt hỗ trợ giảm khô mắt.',85000,220,0,N'Carboxymethylcellulose sodium',N'Nhãn khoa',N'Dung dịch nhỏ mắt',N'0.5%',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091027') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091027',N'Antacid Suspension (Chai 200ml)',N'Hỗn dịch trung hòa acid dạ dày.',68000,210,0,N'Aluminium hydroxide + Magnesium hydroxide',N'Tiêu hóa',N'Hỗn dịch uống',N'200ml',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091028') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091028',N'Men vi sinh Probiotic (Hộp 20 gói)',N'Sản phẩm bổ sung lợi khuẩn đường ruột.',125000,240,0,N'Lactobacillus + Bifidobacterium',N'Tiêu hóa',N'Bột pha uống',N'20 gói',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091029') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091029',N'Multivitamin Daily (Hộp 30 viên)',N'Viên bổ sung vitamin và khoáng chất tổng hợp.',135000,280,0,N'Multivitamins + Minerals',N'Vitamin - Khoáng chất',N'Viên nén',N'30 viên',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091030') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091030',N'Băng cá nhân kháng khuẩn (Hộp 50 miếng)',N'Băng dán vết thương nhỏ dùng một lần.',49000,650,0,NULL,N'Vật tư y tế',N'Khác',NULL,0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091031') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091031',N'Gạc y tế vô trùng 10x10cm (Gói 10 miếng)',N'Gạc vô trùng dùng chăm sóc vết thương.',26000,800,0,NULL,N'Vật tư y tế',N'Khác',N'10x10cm',0);
    IF NOT EXISTS (SELECT 1 FROM products WHERE id='00000000-0000-0000-0000-000000091032') INSERT INTO products(id,name,description,price,stock_quantity,is_prescription_required,active_ingredient,therapeutic_group,dosage_form,strength,prescription_required) VALUES('00000000-0000-0000-0000-000000091032',N'Khẩu trang N95 (Hộp 10 cái)',N'Khẩu trang lọc bụi mịn dùng trong môi trường cần bảo vệ hô hấp.',120000,350,0,NULL,N'Vật tư y tế',N'Khác',N'N95',0);

    -- 5. MEDICATIONS
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071011') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071011',N'Aspirin 81mg',N'Acetylsalicylic acid',N'Tim mạch');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071012') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071012',N'Loratadine 10mg',N'Loratadine',N'Kháng dị ứng');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071013') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071013',N'Chlorpheniramine 4mg',N'Chlorpheniramine maleate',N'Kháng dị ứng');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071014') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071014',N'Dextromethorphan',N'Dextromethorphan',N'Hô hấp');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071015') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071015',N'Guaifenesin',N'Guaifenesin',N'Hô hấp');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071016') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071016',N'Antacid',N'Aluminium hydroxide + Magnesium hydroxide',N'Tiêu hóa');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071017') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071017',N'Probiotic',N'Lactobacillus + Bifidobacterium',N'Tiêu hóa');
    IF NOT EXISTS (SELECT 1 FROM medications WHERE id='00000000-0000-0000-0000-000000071018') INSERT INTO medications(id,name,generic_name,category) VALUES('00000000-0000-0000-0000-000000071018',N'Calcium + Vitamin D3',N'Calcium carbonate + Cholecalciferol',N'Vitamin - Khoáng chất');

    -- Liên kết các Product bổ sung với Medication tương ứng.
    UPDATE products
    SET medication_id = CASE id
        WHEN '00000000-0000-0000-0000-000000091013' THEN '00000000-0000-0000-0000-000000071011'
        WHEN '00000000-0000-0000-0000-000000091015' THEN '00000000-0000-0000-0000-000000071012'
        WHEN '00000000-0000-0000-0000-000000091016' THEN '00000000-0000-0000-0000-000000071013'
        WHEN '00000000-0000-0000-0000-000000091018' THEN '00000000-0000-0000-0000-000000071018'
        WHEN '00000000-0000-0000-0000-000000091023' THEN '00000000-0000-0000-0000-000000071014'
        WHEN '00000000-0000-0000-0000-000000091024' THEN '00000000-0000-0000-0000-000000071015'
        WHEN '00000000-0000-0000-0000-000000091027' THEN '00000000-0000-0000-0000-000000071016'
        WHEN '00000000-0000-0000-0000-000000091028' THEN '00000000-0000-0000-0000-000000071017'
    END
    WHERE id IN (
        '00000000-0000-0000-0000-000000091013',
        '00000000-0000-0000-0000-000000091015',
        '00000000-0000-0000-0000-000000091016',
        '00000000-0000-0000-0000-000000091018',
        '00000000-0000-0000-0000-000000091023',
        '00000000-0000-0000-0000-000000091024',
        '00000000-0000-0000-0000-000000091027',
        '00000000-0000-0000-0000-000000091028'
    );

    -- 6. HEALTH METRICS + GOALS
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151001') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151001','00000000-0000-0000-0000-000000011006',1,66,'2026-08-16T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151002') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151002','00000000-0000-0000-0000-000000011006',2,74,'2026-08-16T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151003') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151003','00000000-0000-0000-0000-000000011006',6,98,'2026-08-16T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161006') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161006','00000000-0000-0000-0000-000000011006',1,63,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001006',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151004') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151004','00000000-0000-0000-0000-000000011007',1,50,'2026-08-17T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151005') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151005','00000000-0000-0000-0000-000000011007',2,75,'2026-08-17T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151006') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151006','00000000-0000-0000-0000-000000011007',6,99,'2026-08-17T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161007') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161007','00000000-0000-0000-0000-000000011007',1,47,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001007',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151007') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151007','00000000-0000-0000-0000-000000011008',1,80,'2026-08-18T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151008') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151008','00000000-0000-0000-0000-000000011008',2,76,'2026-08-18T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151009') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151009','00000000-0000-0000-0000-000000011008',6,96,'2026-08-18T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161008') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161008','00000000-0000-0000-0000-000000011008',1,77,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001008',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151010') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151010','00000000-0000-0000-0000-000000011009',1,55,'2026-08-19T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151011') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151011','00000000-0000-0000-0000-000000011009',2,77,'2026-08-19T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151012') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151012','00000000-0000-0000-0000-000000011009',6,97,'2026-08-19T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161009') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161009','00000000-0000-0000-0000-000000011009',1,52,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001009',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151013') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151013','00000000-0000-0000-0000-000000011010',1,76,'2026-08-20T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151014') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151014','00000000-0000-0000-0000-000000011010',2,78,'2026-08-20T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151015') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151015','00000000-0000-0000-0000-000000011010',6,98,'2026-08-20T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161010') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161010','00000000-0000-0000-0000-000000011010',1,73,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001010',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151016') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151016','00000000-0000-0000-0000-000000011011',1,52,'2026-08-21T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151017') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151017','00000000-0000-0000-0000-000000011011',2,79,'2026-08-21T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151018') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151018','00000000-0000-0000-0000-000000011011',6,99,'2026-08-21T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161011') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161011','00000000-0000-0000-0000-000000011011',1,49,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001011',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151019') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151019','00000000-0000-0000-0000-000000011012',1,73,'2026-08-22T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151020') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151020','00000000-0000-0000-0000-000000011012',2,68,'2026-08-22T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151021') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151021','00000000-0000-0000-0000-000000011012',6,96,'2026-08-22T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161012') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161012','00000000-0000-0000-0000-000000011012',1,70,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001012',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151022') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151022','00000000-0000-0000-0000-000000011013',1,61,'2026-08-23T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151023') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151023','00000000-0000-0000-0000-000000011013',2,69,'2026-08-23T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151024') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151024','00000000-0000-0000-0000-000000011013',6,97,'2026-08-23T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161013') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161013','00000000-0000-0000-0000-000000011013',1,58,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001013',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151025') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151025','00000000-0000-0000-0000-000000011014',1,70,'2026-08-24T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151026') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151026','00000000-0000-0000-0000-000000011014',2,70,'2026-08-24T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151027') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151027','00000000-0000-0000-0000-000000011014',6,98,'2026-08-24T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161014') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161014','00000000-0000-0000-0000-000000011014',1,67,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001014',N'patient');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151028') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151028','00000000-0000-0000-0000-000000011015',1,54,'2026-08-25T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151029') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151029','00000000-0000-0000-0000-000000011015',2,71,'2026-08-25T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_metrics WHERE id='00000000-0000-0000-0000-000000151030') INSERT INTO health_metrics(id,patient_id,metric_type_id,value,recorded_at) VALUES('00000000-0000-0000-0000-000000151030','00000000-0000-0000-0000-000000011015',6,99,'2026-08-25T07:30:00');
    IF NOT EXISTS (SELECT 1 FROM health_goals WHERE id='00000000-0000-0000-0000-000000161015') INSERT INTO health_goals(id,patient_id,metric_type_id,target_value,start_date,end_date,status,created_by_user_id,created_by_role) VALUES('00000000-0000-0000-0000-000000161015','00000000-0000-0000-0000-000000011015',1,51,'2026-08-10','2026-12-31',N'in_progress','00000000-0000-0000-0000-000000001015',N'patient');

    -- 7. SYMPTOM LOGS
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131005') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131005','00000000-0000-0000-0000-000000011006',N'Hắt hơi, nghẹt mũi vào buổi sáng, không sốt.',N'Viêm mũi dị ứng',N'LOW',N'Theo dõi triệu chứng và hạn chế tiếp xúc dị nguyên.','2026-08-15T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131006') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131006','00000000-0000-0000-0000-000000011007',N'Ngứa mắt, chảy nước mắt khi ra ngoài trời.',N'Dị ứng theo mùa',N'LOW',N'Hạn chế tác nhân nghi ngờ và theo dõi.','2026-08-16T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131007') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131007','00000000-0000-0000-0000-000000011008',N'Đau đầu nhẹ sau giờ làm việc, không có dấu hiệu thần kinh khác.',N'Đau đầu do căng thẳng',N'LOW',N'Nghỉ ngơi và đi khám nếu đau tăng.','2026-08-17T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131008') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131008','00000000-0000-0000-0000-000000011009',N'Khô mắt sau khi dùng máy tính nhiều giờ.',N'Khô mắt',N'LOW',N'Nghỉ mắt định kỳ và khám nếu kéo dài.','2026-08-18T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131009') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131009','00000000-0000-0000-0000-000000011010',N'Huyết áp tại nhà cao kèm đau đầu.',N'Tăng huyết áp cần đánh giá',N'HIGH',N'Nên được cơ sở y tế đánh giá sớm.','2026-08-19T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131010') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131010','00000000-0000-0000-0000-000000011011',N'Đầy bụng sau ăn, không nôn.',N'Rối loạn tiêu hóa nhẹ',N'LOW',N'Theo dõi chế độ ăn và đi khám nếu kéo dài.','2026-08-20T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131011') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131011','00000000-0000-0000-0000-000000011012',N'Đau thượng vị tái diễn và buồn nôn.',N'Viêm dạ dày',N'LOW',N'Nên đặt lịch khám tiêu hóa.','2026-08-21T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131012') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131012','00000000-0000-0000-0000-000000011013',N'Sổ mũi, đau họng nhẹ và mệt mỏi.',N'Cảm cúm thông thường',N'LOW',N'Nghỉ ngơi và theo dõi dấu hiệu nặng.','2026-08-22T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131013') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131013','00000000-0000-0000-0000-000000011014',N'Khó thở tăng khi vận động, cảm giác tức ngực.',N'Khó thở cần đánh giá',N'HIGH',N'Cần đánh giá y tế sớm.','2026-08-23T20:00:00');
    IF NOT EXISTS (SELECT 1 FROM symptom_logs WHERE id='00000000-0000-0000-0000-000000131014') INSERT INTO symptom_logs(id,patient_id,symptoms_description,predicted_disease,severity_level,ai_advice,created_at) VALUES('00000000-0000-0000-0000-000000131014','00000000-0000-0000-0000-000000011015',N'Ho khan từng cơn, không sốt cao.',N'Ho khan nhẹ',N'LOW',N'Theo dõi và đi khám nếu kéo dài.','2026-08-24T20:00:00');

    -- 8. CHAT + APPOINTMENTS
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041005') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041005','00000000-0000-0000-0000-000000011006','00000000-0000-0000-0000-000000031005','00000000-0000-0000-0000-000000131005',N'active','2026-08-17T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171001') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171001','00000000-0000-0000-0000-000000041005','00000000-0000-0000-0000-000000001006',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-17T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171002') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171002','00000000-0000-0000-0000-000000041005','00000000-0000-0000-0000-000000002005',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-17T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051006') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051006','00000000-0000-0000-0000-000000041005','00000000-0000-0000-0000-000000011006','00000000-0000-0000-0000-000000031005','2026-08-24T09:00:00',N'online_consult',N'confirmed','2026-08-17T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041006') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041006','00000000-0000-0000-0000-000000011007','00000000-0000-0000-0000-000000031004','00000000-0000-0000-0000-000000131006',N'active','2026-08-18T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171003') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171003','00000000-0000-0000-0000-000000041006','00000000-0000-0000-0000-000000001007',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-18T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171004') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171004','00000000-0000-0000-0000-000000041006','00000000-0000-0000-0000-000000002004',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-18T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051007') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051007','00000000-0000-0000-0000-000000041006','00000000-0000-0000-0000-000000011007','00000000-0000-0000-0000-000000031004','2026-08-25T10:00:00',N'clinic_visit',N'pending','2026-08-18T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041007') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041007','00000000-0000-0000-0000-000000011008','00000000-0000-0000-0000-000000031003','00000000-0000-0000-0000-000000131007',N'active','2026-08-19T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171005') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171005','00000000-0000-0000-0000-000000041007','00000000-0000-0000-0000-000000001008',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-19T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171006') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171006','00000000-0000-0000-0000-000000041007','00000000-0000-0000-0000-000000002003',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-19T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051008') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051008','00000000-0000-0000-0000-000000041007','00000000-0000-0000-0000-000000011008','00000000-0000-0000-0000-000000031003','2026-08-26T11:00:00',N'online_consult',N'completed','2026-08-19T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041008') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041008','00000000-0000-0000-0000-000000011009','00000000-0000-0000-0000-000000031004','00000000-0000-0000-0000-000000131008',N'active','2026-08-20T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171007') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171007','00000000-0000-0000-0000-000000041008','00000000-0000-0000-0000-000000001009',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-20T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171008') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171008','00000000-0000-0000-0000-000000041008','00000000-0000-0000-0000-000000002004',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-20T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051009') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051009','00000000-0000-0000-0000-000000041008','00000000-0000-0000-0000-000000011009','00000000-0000-0000-0000-000000031004','2026-08-27T12:00:00',N'clinic_visit',N'confirmed','2026-08-20T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041009') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041009','00000000-0000-0000-0000-000000011010','00000000-0000-0000-0000-000000031001','00000000-0000-0000-0000-000000131009',N'active','2026-08-21T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171009') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171009','00000000-0000-0000-0000-000000041009','00000000-0000-0000-0000-000000001010',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-21T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171010') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171010','00000000-0000-0000-0000-000000041009','00000000-0000-0000-0000-000000002001',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-21T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051010') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051010','00000000-0000-0000-0000-000000041009','00000000-0000-0000-0000-000000011010','00000000-0000-0000-0000-000000031001','2026-08-28T13:00:00',N'online_consult',N'completed','2026-08-21T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041010') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041010','00000000-0000-0000-0000-000000011011','00000000-0000-0000-0000-000000031006','00000000-0000-0000-0000-000000131010',N'active','2026-08-22T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171011') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171011','00000000-0000-0000-0000-000000041010','00000000-0000-0000-0000-000000001011',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-22T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171012') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171012','00000000-0000-0000-0000-000000041010','00000000-0000-0000-0000-000000002006',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-22T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051011') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051011','00000000-0000-0000-0000-000000041010','00000000-0000-0000-0000-000000011011','00000000-0000-0000-0000-000000031006','2026-08-29T09:00:00',N'clinic_visit',N'pending','2026-08-22T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041011') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041011','00000000-0000-0000-0000-000000011012','00000000-0000-0000-0000-000000031006','00000000-0000-0000-0000-000000131011',N'active','2026-08-23T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171013') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171013','00000000-0000-0000-0000-000000041011','00000000-0000-0000-0000-000000001012',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-23T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171014') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171014','00000000-0000-0000-0000-000000041011','00000000-0000-0000-0000-000000002006',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-23T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051012') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051012','00000000-0000-0000-0000-000000041011','00000000-0000-0000-0000-000000011012','00000000-0000-0000-0000-000000031006','2026-08-30T10:00:00',N'online_consult',N'completed','2026-08-23T08:40:00');
    IF NOT EXISTS (SELECT 1 FROM chat_sessions WHERE id='00000000-0000-0000-0000-000000041012') INSERT INTO chat_sessions(id,patient_id,doctor_id,ai_symptom_log_id,status,created_at) VALUES('00000000-0000-0000-0000-000000041012','00000000-0000-0000-0000-000000011013','00000000-0000-0000-0000-000000031002','00000000-0000-0000-0000-000000131012',N'active','2026-08-24T08:30:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171015') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171015','00000000-0000-0000-0000-000000041012','00000000-0000-0000-0000-000000001013',N'Tôi muốn được bác sĩ tư vấn thêm về triệu chứng gần đây.',1,'2026-08-24T08:31:00');
    IF NOT EXISTS (SELECT 1 FROM chat_messages WHERE id='00000000-0000-0000-0000-000000171016') INSERT INTO chat_messages(id,session_id,sender_id,message,is_read,sent_at) VALUES('00000000-0000-0000-0000-000000171016','00000000-0000-0000-0000-000000041012','00000000-0000-0000-0000-000000002002',N'Tôi đã nhận được thông tin. Bạn hãy mô tả thêm thời điểm và mức độ triệu chứng.',0,'2026-08-24T08:35:00');
    IF NOT EXISTS (SELECT 1 FROM appointments WHERE id='00000000-0000-0000-0000-000000051013') INSERT INTO appointments(id,chat_session_id,patient_id,doctor_id,scheduled_at,type,status,created_at) VALUES('00000000-0000-0000-0000-000000051013','00000000-0000-0000-0000-000000041012','00000000-0000-0000-0000-000000011013','00000000-0000-0000-0000-000000031002','2026-08-31T11:00:00',N'clinic_visit',N'confirmed','2026-08-24T08:40:00');

    -- 9. NOTIFICATIONS
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201001') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201001','00000000-0000-0000-0000-000000001006',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201002') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201002','00000000-0000-0000-0000-000000001007',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201003') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201003','00000000-0000-0000-0000-000000001008',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201004') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201004','00000000-0000-0000-0000-000000001009',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201005') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201005','00000000-0000-0000-0000-000000001010',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201006') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201006','00000000-0000-0000-0000-000000001011',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201007') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201007','00000000-0000-0000-0000-000000001012',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201008') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201008','00000000-0000-0000-0000-000000001013',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201009') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201009','00000000-0000-0000-0000-000000001014',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201010') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201010','00000000-0000-0000-0000-000000001015',N'appointment_reminder',N'Nhắc lịch và cập nhật Blue Crown',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201011') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201011','00000000-0000-0000-0000-000000002004',N'new_appointment',N'Lịch khám mới',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201012') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201012','00000000-0000-0000-0000-000000002005',N'new_appointment',N'Lịch khám mới',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201013') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201013','00000000-0000-0000-0000-000000002006',N'new_appointment',N'Lịch khám mới',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201014') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201014','00000000-0000-0000-0000-000000004003',N'inventory_notice',N'Thông báo hệ thống',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');
    IF NOT EXISTS (SELECT 1 FROM notifications WHERE id='00000000-0000-0000-0000-000000201015') INSERT INTO notifications(id,user_id,type,title,message,is_read,created_at) VALUES('00000000-0000-0000-0000-000000201015','00000000-0000-0000-0000-000000003001',N'inventory_notice',N'Thông báo hệ thống',N'Dữ liệu demo bổ sung để kiểm thử danh sách thông báo và phân trang.',0,'2026-08-21T08:00:00');

    -- 10. ECOMMERCE ORDERS + ITEMS
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101005') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101005','00000000-0000-0000-0000-000000001006',NULL,N'Địa chỉ giao hàng demo 5, TP.HCM',100000,N'cod',N'pending',N'processing',NULL,'2026-08-15T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221001') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221001','00000000-0000-0000-0000-000000101005','00000000-0000-0000-0000-000000091015',1,28000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221002') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221002','00000000-0000-0000-0000-000000101005','00000000-0000-0000-0000-000000091025',1,72000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101006') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101006','00000000-0000-0000-0000-000000001007',NULL,N'Địa chỉ giao hàng demo 6, TP.HCM',107000,N'cod',N'pending',N'processing',NULL,'2026-08-16T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221003') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221003','00000000-0000-0000-0000-000000101006','00000000-0000-0000-0000-000000091016',1,22000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221004') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221004','00000000-0000-0000-0000-000000101006','00000000-0000-0000-0000-000000091026',1,85000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101007') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101007','00000000-0000-0000-0000-000000001008',NULL,N'Địa chỉ giao hàng demo 7, TP.HCM',167000,N'cod',N'pending',N'processing',NULL,'2026-08-17T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221005') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221005','00000000-0000-0000-0000-000000101007','00000000-0000-0000-0000-000000091029',1,135000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221006') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221006','00000000-0000-0000-0000-000000101007','00000000-0000-0000-0000-000000091020',1,32000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101008') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101008','00000000-0000-0000-0000-000000001009',NULL,N'Địa chỉ giao hàng demo 8, TP.HCM',171000,N'cod',N'pending',N'processing',NULL,'2026-08-18T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221007') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221007','00000000-0000-0000-0000-000000101008','00000000-0000-0000-0000-000000091018',1,145000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221008') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221008','00000000-0000-0000-0000-000000101008','00000000-0000-0000-0000-000000091031',1,26000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101009') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101009','00000000-0000-0000-0000-000000001010',NULL,N'Địa chỉ giao hàng demo 9, TP.HCM',91000,N'cod',N'pending',N'processing',NULL,'2026-08-19T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221009') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221009','00000000-0000-0000-0000-000000101009','00000000-0000-0000-0000-000000091013',1,39000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221010') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221010','00000000-0000-0000-0000-000000101009','00000000-0000-0000-0000-000000091014',1,52000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101010') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101010','00000000-0000-0000-0000-000000001011',NULL,N'Địa chỉ giao hàng demo 10, TP.HCM',214000,N'cod',N'pending',N'processing',NULL,'2026-08-20T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221011') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221011','00000000-0000-0000-0000-000000101010','00000000-0000-0000-0000-000000091018',1,145000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221012') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221012','00000000-0000-0000-0000-000000101010','00000000-0000-0000-0000-000000091021',1,69000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101011') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101011','00000000-0000-0000-0000-000000001012',NULL,N'Địa chỉ giao hàng demo 11, TP.HCM',146000,N'cod',N'pending',N'processing',NULL,'2026-08-21T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221013') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221013','00000000-0000-0000-0000-000000101011','00000000-0000-0000-0000-000000091022',1,89000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221014') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221014','00000000-0000-0000-0000-000000101011','00000000-0000-0000-0000-000000091023',1,57000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101012') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101012','00000000-0000-0000-0000-000000001013',NULL,N'Địa chỉ giao hàng demo 12, TP.HCM',139000,N'cod',N'pending',N'processing',NULL,'2026-08-22T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221015') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221015','00000000-0000-0000-0000-000000101012','00000000-0000-0000-0000-000000091024',1,61000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221016') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221016','00000000-0000-0000-0000-000000101012','00000000-0000-0000-0000-000000091017',1,78000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101013') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101013',NULL,N'0909000013',N'Địa chỉ giao hàng demo 13, TP.HCM',169000,N'cod',N'pending',N'processing',NULL,'2026-08-23T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221017') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221017','00000000-0000-0000-0000-000000101013','00000000-0000-0000-0000-000000091030',1,49000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221018') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221018','00000000-0000-0000-0000-000000101013','00000000-0000-0000-0000-000000091032',1,120000);
    IF NOT EXISTS (SELECT 1 FROM ecommerce_orders WHERE id='00000000-0000-0000-0000-000000101014') INSERT INTO ecommerce_orders(id,user_id,guest_phone,shipping_address,total_amount,payment_method,payment_status,order_status,prescription_id,created_at) VALUES('00000000-0000-0000-0000-000000101014',NULL,N'0909000014',N'Địa chỉ giao hàng demo 14, TP.HCM',193000,N'cod',N'pending',N'processing',NULL,'2026-08-24T10:00:00');
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221019') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221019','00000000-0000-0000-0000-000000101014','00000000-0000-0000-0000-000000091027',1,68000);
    IF NOT EXISTS (SELECT 1 FROM order_items WHERE id='00000000-0000-0000-0000-000000221020') INSERT INTO order_items(id,order_id,product_id,quantity,unit_price) VALUES('00000000-0000-0000-0000-000000221020','00000000-0000-0000-0000-000000101014','00000000-0000-0000-0000-000000091028',1,125000);

    -- 11. SUPPLIERS + INVENTORY RECEIPTS + RECEIPT DETAILS
    IF NOT EXISTS (SELECT 1 FROM suppliers WHERE id='00000000-0000-0000-0000-000000111004') INSERT INTO suppliers(id,supplier_name,contact_phone,gdp_certified,created_at) VALUES('00000000-0000-0000-0000-000000111004',N'Công ty CP Traphaco',N'02436830001',1,'2025-08-04');
    IF NOT EXISTS (SELECT 1 FROM suppliers WHERE id='00000000-0000-0000-0000-000000111005') INSERT INTO suppliers(id,supplier_name,contact_phone,gdp_certified,created_at) VALUES('00000000-0000-0000-0000-000000111005',N'Công ty CP Dược phẩm Imexpharm',N'02773851941',1,'2025-08-04');
    IF NOT EXISTS (SELECT 1 FROM suppliers WHERE id='00000000-0000-0000-0000-000000111006') INSERT INTO suppliers(id,supplier_name,contact_phone,gdp_certified,created_at) VALUES('00000000-0000-0000-0000-000000111006',N'Công ty CP Pymepharco',N'02573829090',1,'2025-08-04');
    IF NOT EXISTS (SELECT 1 FROM inventory_receipts WHERE id='00000000-0000-0000-0000-000000121004') INSERT INTO inventory_receipts(id,supplier_id,created_by,approved_by,total_cost,receipt_date,status) VALUES('00000000-0000-0000-0000-000000121004','00000000-0000-0000-0000-000000111004','00000000-0000-0000-0000-000000004003','00000000-0000-0000-0000-000000003001',6200000,'2026-08-21T09:00:00',N'approved');
    IF NOT EXISTS (SELECT 1 FROM inventory_receipts WHERE id='00000000-0000-0000-0000-000000121005') INSERT INTO inventory_receipts(id,supplier_id,created_by,approved_by,total_cost,receipt_date,status) VALUES('00000000-0000-0000-0000-000000121005','00000000-0000-0000-0000-000000111005','00000000-0000-0000-0000-000000004001','00000000-0000-0000-0000-000000003001',7800000,'2026-08-21T09:00:00',N'approved');
    IF NOT EXISTS (SELECT 1 FROM inventory_receipts WHERE id='00000000-0000-0000-0000-000000121006') INSERT INTO inventory_receipts(id,supplier_id,created_by,approved_by,total_cost,receipt_date,status) VALUES('00000000-0000-0000-0000-000000121006','00000000-0000-0000-0000-000000111006','00000000-0000-0000-0000-000000004002',NULL,5100000,'2026-08-21T09:00:00',N'pending_approval');
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231001') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231001','00000000-0000-0000-0000-000000121004','00000000-0000-0000-0000-000000091015',N'LOR2026D01','2028-02-01',1800,12000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231002') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231002','00000000-0000-0000-0000-000000121004','00000000-0000-0000-0000-000000091019',N'PVI2026D02','2028-04-15',1500,18000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231003') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231003','00000000-0000-0000-0000-000000121004','00000000-0000-0000-0000-000000091020',N'ALC2026D03','2029-01-01',3000,10000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231004') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231004','00000000-0000-0000-0000-000000121005','00000000-0000-0000-0000-000000091023',N'DEX2026E01','2027-12-01',1200,31000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231005') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231005','00000000-0000-0000-0000-000000121005','00000000-0000-0000-0000-000000091024',N'GUA2026E02','2028-03-10',1200,33000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231006') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231006','00000000-0000-0000-0000-000000121005','00000000-0000-0000-0000-000000091028',N'PRO2026E03','2027-11-20',900,68000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231007') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231007','00000000-0000-0000-0000-000000121006','00000000-0000-0000-0000-000000091029',N'MVT2026F01','2028-06-01',1000,75000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231008') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231008','00000000-0000-0000-0000-000000121006','00000000-0000-0000-0000-000000091030',N'BAN2026F02','2029-06-01',2500,15000);
    IF NOT EXISTS (SELECT 1 FROM receipt_details WHERE id='00000000-0000-0000-0000-000000231009') INSERT INTO receipt_details(id,receipt_id,product_id,batch_number,expiration_date,quantity_imported,import_price) VALUES('00000000-0000-0000-0000-000000231009','00000000-0000-0000-0000-000000121006','00000000-0000-0000-0000-000000091032',N'N952026F03','2029-01-01',1300,65000);

    COMMIT TRANSACTION;

    PRINT N'Đã bổ sung dữ liệu demo BLUE_CROWN thành công.';
    SELECT N'users' table_name, COUNT(*) row_count FROM users
    UNION ALL SELECT N'products', COUNT(*) FROM products
    UNION ALL SELECT N'health_metrics', COUNT(*) FROM health_metrics
    UNION ALL SELECT N'appointments', COUNT(*) FROM appointments
    UNION ALL SELECT N'ecommerce_orders', COUNT(*) FROM ecommerce_orders
    UNION ALL SELECT N'order_items', COUNT(*) FROM order_items
    UNION ALL SELECT N'notifications', COUNT(*) FROM notifications
    UNION ALL SELECT N'symptom_logs', COUNT(*) FROM symptom_logs
    UNION ALL SELECT N'inventory_receipts', COUNT(*) FROM inventory_receipts;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

UPDATE products SET image_url = N'/images/products/paracetamol.jpg'
WHERE id = '00000000-0000-0000-0000-000000091001';

UPDATE products SET image_url = N'/images/products/amoxicillin.jpg'
WHERE id = '00000000-0000-0000-0000-000000091002';

UPDATE products SET image_url = N'/images/products/metformin.jpg'
WHERE id = '00000000-0000-0000-0000-000000091003';

UPDATE products SET image_url = N'/images/products/losartan.jpg'
WHERE id = '00000000-0000-0000-0000-000000091004';

UPDATE products SET image_url = N'/images/products/vitamin-c.jpg'
WHERE id = '00000000-0000-0000-0000-000000091005';

UPDATE products SET image_url = N'/images/products/omeprazole.jpg'
WHERE id = '00000000-0000-0000-0000-000000091006';

UPDATE products SET image_url = N'/images/products/cetirizine.jpg'
WHERE id = '00000000-0000-0000-0000-000000091007';

UPDATE products SET image_url = N'/images/products/ventolin.jpg'
WHERE id = '00000000-0000-0000-0000-000000091008';

UPDATE products SET image_url = N'/images/products/nacl.jpg'
WHERE id = '00000000-0000-0000-0000-000000091009';

UPDATE products SET image_url = N'/images/products/medical-mask.jpg'
WHERE id = '00000000-0000-0000-0000-000000091010';

UPDATE products SET image_url = N'/images/products/ibuprofen.jpg'
WHERE id = '00000000-0000-0000-0000-000000091011';

UPDATE products SET image_url = N'/images/products/oresol.jpg'
WHERE id = '00000000-0000-0000-0000-000000091012';

UPDATE products SET image_url = N'/images/products/aspirin.jpg'
WHERE id = '00000000-0000-0000-0000-000000091013';

UPDATE products SET image_url = N'/images/products/diclofenac.jpg'
WHERE id = '00000000-0000-0000-0000-000000091014';

UPDATE products SET image_url = N'/images/products/loratadine.jpg'
WHERE id = '00000000-0000-0000-0000-000000091015';

UPDATE products SET image_url = N'/images/products/chlorpheniramine.jpg'
WHERE id = '00000000-0000-0000-0000-000000091016';

UPDATE products SET image_url = N'/images/products/zinc.jpg'
WHERE id = '00000000-0000-0000-0000-000000091017';

UPDATE products SET image_url = N'/images/products/calcium&vitamind3.jpg'
WHERE id = '00000000-0000-0000-0000-000000091018';

UPDATE products SET image_url = N'/images/products/povidoneIodine.jpg'
WHERE id = '00000000-0000-0000-0000-000000091019';

UPDATE products SET image_url = N'/images/products/ethanol.jpg'
WHERE id = '00000000-0000-0000-0000-000000091020';

UPDATE products SET image_url = N'/images/products/gel.jpg'
WHERE id = '00000000-0000-0000-0000-000000091021';

UPDATE products SET image_url = N'/images/products/thermometer.jpg'
WHERE id = '00000000-0000-0000-0000-000000091022';

UPDATE products SET image_url = N'/images/products/dextromethorphansyrup.jpg'
WHERE id = '00000000-0000-0000-0000-000000091023';

UPDATE products SET image_url = N'/images/products/guaifenesinsyrup.jpg'
WHERE id = '00000000-0000-0000-0000-000000091024';

UPDATE products SET image_url = N'/images/products/xisat.jpg'
WHERE id = '00000000-0000-0000-0000-000000091025';

UPDATE products SET image_url = N'/images/products/sanlain.jpg'
WHERE id = '00000000-0000-0000-0000-000000091026';

UPDATE products SET image_url = N'/images/products/antacidsuspension.jpg'
WHERE id = '00000000-0000-0000-0000-000000091027';

UPDATE products SET image_url = N'/images/products/probiotic.jpg'
WHERE id = '00000000-0000-0000-0000-000000091028';

UPDATE products SET image_url = N'/images/products/multivitamindaily.jpg'
WHERE id = '00000000-0000-0000-0000-000000091029';

UPDATE products SET image_url = N'/images/products/antibacterialbandage.jpg'
WHERE id = '00000000-0000-0000-0000-000000091030';

UPDATE products SET image_url = N'/images/products/sterilemedicalgauze.jpg'
WHERE id = '00000000-0000-0000-0000-000000091031';

UPDATE products SET image_url = N'/images/products/n95mask.jpg'
WHERE id = '00000000-0000-0000-0000-000000091032';


USE BLUE_CROWN;
GO

SET NOCOUNT ON;

PRINT N'===== 1. KIỂM TRA CỘT/INDEX QUAN TRỌNG =====';

SELECT
    COL_LENGTH('prescriptions', 'appointment_id') AS prescription_appointment_id,
    COL_LENGTH('prescriptions', 'medical_record_id') AS prescription_medical_record_id,
    COL_LENGTH('prescriptions', 'diagnosis') AS prescription_diagnosis,
    COL_LENGTH('health_goals', 'created_by_user_id') AS health_goal_created_by_user_id,
    COL_LENGTH('health_goals', 'created_by_role') AS health_goal_created_by_role,
    COL_LENGTH('products', 'medication_id') AS product_medication_id;

SELECT name, is_unique
FROM sys.indexes
WHERE object_id = OBJECT_ID('prescriptions')
  AND name = 'UX_prescriptions_appointment_id';

PRINT N'===== 2. KIỂM TRA PRESCRIPTION =====';

SELECT
    id,
    appointment_id,
    medical_record_id,
    patient_id,
    doctor_id,
    diagnosis,
    status,
    created_at
FROM prescriptions
ORDER BY created_at;

SELECT appointment_id, COUNT(*) AS prescription_count
FROM prescriptions
GROUP BY appointment_id
HAVING COUNT(*) > 1;

SELECT COUNT(*) AS invalid_prescription_rows
FROM prescriptions
WHERE appointment_id IS NULL
   OR status NOT IN ('pending', 'approved', 'dispensed', 'cancelled');

PRINT N'===== 3. KIỂM TRA HEALTH GOAL =====';

SELECT COUNT(*) AS health_goal_missing_creator
FROM health_goals
WHERE created_by_user_id IS NULL
   OR created_by_role NOT IN ('patient', 'doctor');

PRINT N'===== 4. KIỂM TRA ECOMMERCE COD =====';

SELECT
    id,
    payment_method,
    payment_status,
    order_status,
    prescription_id
FROM ecommerce_orders
ORDER BY created_at;

SELECT COUNT(*) AS non_cod_ecommerce_orders
FROM ecommerce_orders
WHERE LOWER(payment_method) <> 'cod';

PRINT N'===== 5. KIỂM TRA PRODUCT -> MEDICATION =====';

SELECT
    p.id,
    p.name AS product_name,
    p.medication_id,
    m.name AS medication_name,
    p.stock_quantity,
    p.is_prescription_required,
    p.prescription_required
FROM products p
LEFT JOIN medications m ON m.id = p.medication_id
ORDER BY p.name;

SELECT COUNT(*) AS prescription_flag_mismatch
FROM products
WHERE ISNULL(is_prescription_required, 0) <> prescription_required;

PRINT N'===== 6. KIỂM TRA DỮ LIỆU TỔNG =====';

SELECT N'users' AS table_name, COUNT(*) AS row_count FROM users
UNION ALL SELECT N'patient_profiles', COUNT(*) FROM patient_profiles
UNION ALL SELECT N'doctor_profiles', COUNT(*) FROM doctor_profiles
UNION ALL SELECT N'appointments', COUNT(*) FROM appointments
UNION ALL SELECT N'medical_records', COUNT(*) FROM medical_records
UNION ALL SELECT N'prescriptions', COUNT(*) FROM prescriptions
UNION ALL SELECT N'prescription_items', COUNT(*) FROM prescription_items
UNION ALL SELECT N'products', COUNT(*) FROM products
UNION ALL SELECT N'ecommerce_orders', COUNT(*) FROM ecommerce_orders
UNION ALL SELECT N'inventory_receipts', COUNT(*) FROM inventory_receipts
UNION ALL SELECT N'health_goals', COUNT(*) FROM health_goals;

PRINT N'===== HOÀN TẤT KIỂM TRA =====';


USE BLUE_CROWN;
GO

PRINT '===== 1. KIEM TRA PRESCRIPTIONS =====';

SELECT
    c.name AS column_name,
    t.name AS data_type,
    c.max_length,
    c.is_nullable
FROM sys.columns c
JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.prescriptions')
ORDER BY c.column_id;

PRINT '===== 2. PRESCRIPTION KHONG CO APPOINTMENT =====';

SELECT COUNT(*) AS invalid_prescription_rows
FROM prescriptions
WHERE appointment_id IS NULL;

PRINT '===== 3. APPOINTMENT BI TRUNG PRESCRIPTION =====';

SELECT appointment_id, COUNT(*) AS total
FROM prescriptions
GROUP BY appointment_id
HAVING COUNT(*) > 1;

PRINT '===== 4. KIEM TRA HEALTH GOALS =====';

SELECT COUNT(*) AS health_goal_missing_creator
FROM health_goals
WHERE created_by_user_id IS NULL
   OR created_by_role IS NULL;

PRINT '===== 5. ROLE NGUOI TAO HEALTH GOAL KHONG HOP LE =====';

SELECT *
FROM health_goals
WHERE created_by_role NOT IN ('patient', 'doctor');

PRINT '===== 6. KIEM TRA DON HANG KHONG PHAI COD =====';

SELECT COUNT(*) AS non_cod_ecommerce_orders
FROM ecommerce_orders
WHERE LOWER(LTRIM(RTRIM(payment_method))) <> 'cod';

PRINT '===== 7. DANH SACH PAYMENT METHOD CUA ECOMMERCE =====';

SELECT payment_method, COUNT(*) AS total
FROM ecommerce_orders
GROUP BY payment_method;

PRINT '===== 8. KIEM TRA 2 COT PRESCRIPTION FLAG =====';

SELECT COUNT(*) AS prescription_flag_mismatch
FROM products
WHERE ISNULL(is_prescription_required, 0)
   <> ISNULL(prescription_required, 0);

PRINT '===== 9. PRODUCT CHUA GAN MEDICATION =====';

SELECT
    id,
    name,
    medication_id,
    is_prescription_required
FROM products
WHERE medication_id IS NULL
ORDER BY name;

PRINT '===== 10. PRESCRIPTION HIEN TAI =====';

SELECT
    p.id,
    p.appointment_id,
    a.type AS appointment_type,
    a.status AS appointment_status,
    p.medical_record_id,
    p.patient_id,
    p.doctor_id,
    p.status,
    p.diagnosis
FROM prescriptions p
LEFT JOIN appointments a ON a.id = p.appointment_id
ORDER BY p.created_at;

PRINT '===== 11. TONG SO BAN GHI =====';

SELECT 'users' AS table_name, COUNT(*) AS total FROM users
UNION ALL
SELECT 'patient_profiles', COUNT(*) FROM patient_profiles
UNION ALL
SELECT 'doctor_profiles', COUNT(*) FROM doctor_profiles
UNION ALL
SELECT 'appointments', COUNT(*) FROM appointments
UNION ALL
SELECT 'medical_records', COUNT(*) FROM medical_records
UNION ALL
SELECT 'prescriptions', COUNT(*) FROM prescriptions
UNION ALL
SELECT 'products', COUNT(*) FROM products
UNION ALL
SELECT 'ecommerce_orders', COUNT(*) FROM ecommerce_orders
UNION ALL
SELECT 'inventory_receipts', COUNT(*) FROM inventory_receipts;

USE BLUE_CROWN;
GO

-- 1. Thuốc yêu cầu kê đơn nhưng chưa liên kết Medication
SELECT
    id,
    name,
    medication_id,
    is_prescription_required,
    prescription_required
FROM products
WHERE (
        ISNULL(is_prescription_required, 0) = 1
        OR ISNULL(prescription_required, 0) = 1
      )
  AND medication_id IS NULL;

  -- 2. Medication đang có trong Prescription nhưng không có Product tương ứng
SELECT
    pi.medication_id,
    m.name AS medication_name
FROM prescription_items pi
LEFT JOIN medications m
    ON m.id = pi.medication_id
LEFT JOIN products p
    ON p.medication_id = pi.medication_id
WHERE p.id IS NULL
GROUP BY
    pi.medication_id,
    m.name;

	USE BLUE_CROWN;
GO