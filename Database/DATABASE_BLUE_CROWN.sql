CREATE DATABASE BLUE_CROWN;
GO

USE BLUE_CROWN;
GO

-- =========================================================
-- BLUE CROWN - DATABASE FINAL
-- Đồng bộ với mô hình EF Core và nghiệp vụ hiện tại.
-- File này chỉ tạo schema. Dữ liệu mẫu nằm trong file DATA FINAL.
-- Không tự động DROP database để tránh mất dữ liệu ngoài ý muốn.
-- =========================================================


-- =========================================
-- NHÓM 1: NGƯỜI DÙNG & XÁC THỰC
-- =========================================
CREATE TABLE users (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    email NVARCHAR(255) UNIQUE NOT NULL, 
    phone NVARCHAR(20) UNIQUE,
    password_hash NVARCHAR(255) NOT NULL, 
    full_name NVARCHAR(255) NOT NULL,
    date_of_birth DATE, 
    gender NVARCHAR(10), 
    avatar_url NVARCHAR(MAX),
    
    -- Thêm vai trò 'pharmacist' (Dược sĩ kho)
    role NVARCHAR(20) NOT NULL CHECK (role IN ('patient', 'doctor', 'admin', 'pharmacist')),
    
    status NVARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'suspended', 'pending')),
    email_verified_at DATETIME, 
    created_at DATETIME DEFAULT GETDATE(), 
    updated_at DATETIME DEFAULT GETDATE()
);

--UPDATE [BLUE_CROWN].[dbo].[users]
--SET [role] = 'Pharmacist'
--WHERE [email] = 'pharmacist@gmail.com';

--SELECT [email], [role]
--FROM [BLUE_CROWN].[dbo].[users]
--WHERE [email] = 'pharmacist@gmail.com';

-- =========================================
-- NHÓM 2: HỒ SƠ Y TẾ
-- =========================================
CREATE TABLE patient_profiles (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER UNIQUE NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    blood_type NVARCHAR(5), 
    height_cm DECIMAL(5,2),
    WeightKg DECIMAL(5,2) NULL,
    allergies NVARCHAR(MAX), 
    chronic_conditions NVARCHAR(MAX),
    emergency_contact_name NVARCHAR(255), 
    emergency_contact_phone NVARCHAR(20)
);

CREATE TABLE clinics (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(255) NOT NULL, 
    address NVARCHAR(MAX), 
    phone NVARCHAR(20)
);

CREATE TABLE doctor_profiles (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER UNIQUE NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    specialty NVARCHAR(100) NOT NULL, 
    license_number NVARCHAR(100) UNIQUE NOT NULL,
    license_verified BIT DEFAULT 0, 
    bio NVARCHAR(MAX), 
    years_experience INT,
    clinic_id UNIQUEIDENTIFIER REFERENCES clinics(id), 
    consultation_fee DECIMAL(12,2),
    rating_avg DECIMAL(3,2) DEFAULT 0, 
    rating_count INT DEFAULT 0
);

-- =========================================
-- NHÓM 3: CHỈ SỐ SỨC KHỎE
-- =========================================
CREATE TABLE metric_types (
    id INT IDENTITY(1,1) PRIMARY KEY, 
    code NVARCHAR(50) UNIQUE NOT NULL, 
    name NVARCHAR(100) NOT NULL,
    unit NVARCHAR(20) NOT NULL, 
    normal_min DECIMAL(10,2), 
    normal_max DECIMAL(10,2)
);

CREATE TABLE health_metrics (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id) ON DELETE CASCADE,
    metric_type_id INT NOT NULL REFERENCES metric_types(id),
    value DECIMAL(10,2) NOT NULL, 
    recorded_at DATETIME NOT NULL
);

-- =========================================
-- NHÓM 4: HỆ THỐNG TIN NHẮN (CHAT)
-- =========================================
CREATE TABLE chat_sessions (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id),
    doctor_id UNIQUEIDENTIFIER REFERENCES doctor_profiles(id),
    ai_symptom_log_id UNIQUEIDENTIFIER,
    status NVARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'closed')),
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE chat_messages (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    session_id UNIQUEIDENTIFIER NOT NULL REFERENCES chat_sessions(id) ON DELETE CASCADE,
    sender_id UNIQUEIDENTIFIER NOT NULL REFERENCES users(id),
    message NVARCHAR(MAX) NOT NULL, 
    is_read BIT DEFAULT 0, 
    sent_at DATETIME DEFAULT GETDATE()
);

-- =========================================
-- NHÓM 5: ĐẶT LỊCH & BỆNH ÁN (PHÁT SINH TỪ CHAT)
-- =========================================
CREATE TABLE appointments (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    chat_session_id UNIQUEIDENTIFIER REFERENCES chat_sessions(id),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id),
    doctor_id UNIQUEIDENTIFIER NOT NULL REFERENCES doctor_profiles(id),
    scheduled_at DATETIME NOT NULL, 
    type NVARCHAR(20) CHECK (type IN ('online_consult', 'clinic_visit')),
    status NVARCHAR(20) DEFAULT 'pending', 
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE medical_records (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    appointment_id UNIQUEIDENTIFIER REFERENCES appointments(id),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id),
    doctor_id UNIQUEIDENTIFIER NOT NULL REFERENCES doctor_profiles(id),
    diagnosis NVARCHAR(MAX) NOT NULL, 
    notes NVARCHAR(MAX), 
    created_at DATETIME DEFAULT GETDATE()
);

-- =========================================
-- NHÓM 6: ĐƠN THUỐC & NHẮC UỐNG THUỐC
-- =========================================
CREATE TABLE medications (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(255) NOT NULL, 
    generic_name NVARCHAR(255), 
    category NVARCHAR(100)
);

CREATE TABLE prescriptions (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    appointment_id UNIQUEIDENTIFIER NOT NULL,
    medical_record_id UNIQUEIDENTIFIER NULL REFERENCES medical_records(id),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id),
    doctor_id UNIQUEIDENTIFIER NOT NULL REFERENCES doctor_profiles(id),
    diagnosis NVARCHAR(MAX) NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'pending',
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_prescriptions_appointments
        FOREIGN KEY (appointment_id) REFERENCES appointments(id)
);

CREATE UNIQUE INDEX UX_prescriptions_appointment_id
ON prescriptions(appointment_id);
GO

CREATE TABLE prescription_items (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    prescription_id UNIQUEIDENTIFIER NOT NULL REFERENCES prescriptions(id) ON DELETE CASCADE,
    medication_id UNIQUEIDENTIFIER NOT NULL REFERENCES medications(id),
    dosage NVARCHAR(100), 
    frequency_per_day INT, 
    duration_days INT, 
    instructions NVARCHAR(MAX)
);

-- =========================================
-- NHÓM 7, 8, 9: MỤC TIÊU, THANH TOÁN, THÔNG BÁO
-- =========================================
CREATE TABLE health_goals (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id) ON DELETE CASCADE,
    metric_type_id INT NOT NULL REFERENCES metric_types(id),
    target_value DECIMAL(10,2), 
    start_date DATE, 
    end_date DATE, 
    status NVARCHAR(20) DEFAULT 'in_progress',
    created_by_user_id UNIQUEIDENTIFIER NOT NULL,
    created_by_role NVARCHAR(20) NOT NULL,

    CONSTRAINT FK_health_goals_created_by_user
        FOREIGN KEY (created_by_user_id) REFERENCES users(id),
    CONSTRAINT CK_health_goals_created_by_role
        CHECK (created_by_role IN ('patient', 'doctor'))
);
GO

CREATE TABLE payments (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    appointment_id UNIQUEIDENTIFIER NOT NULL REFERENCES appointments(id),
    patient_id UNIQUEIDENTIFIER NOT NULL REFERENCES patient_profiles(id),
    amount DECIMAL(12,2) NOT NULL, 
    platform_fee DECIMAL(12,2) DEFAULT 0,
    status NVARCHAR(20) DEFAULT 'pending', 
    payment_method NVARCHAR(50),
    transaction_ref NVARCHAR(255) UNIQUE, 
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE notifications (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    type NVARCHAR(50), 
    title NVARCHAR(255), 
    message NVARCHAR(MAX), 
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE()
);

-- =========================================
-- NHÓM 10, 11: SẢN PHẨM & E-COMMERCE
-- =========================================
CREATE TABLE products (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(255) NOT NULL, 
    description NVARCHAR(MAX), 
    price DECIMAL(12,2) NOT NULL,
    stock_quantity INT DEFAULT 0, 
    is_prescription_required BIT DEFAULT 0,
	active_ingredient NVARCHAR(200) NULL,
    therapeutic_group NVARCHAR(200) NULL,
    dosage_form NVARCHAR(100) NULL,
    strength NVARCHAR(50) NULL,
    prescription_required BIT NOT NULL DEFAULT 0,
	image_url NVARCHAR(MAX) NULL
);

IF COL_LENGTH('products', 'medication_id') IS NULL
BEGIN
    ALTER TABLE products
    ADD medication_id UNIQUEIDENTIFIER NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_products_medications'
)
BEGIN
    ALTER TABLE products
    ADD CONSTRAINT FK_products_medications
    FOREIGN KEY (medication_id)
    REFERENCES medications(id)
    ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_products_medication_id'
      AND object_id = OBJECT_ID('products')
)
BEGIN
    CREATE INDEX IX_products_medication_id
    ON products(medication_id);
END;

CREATE TABLE drug_alternatives
(
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    product_id UNIQUEIDENTIFIER NOT NULL,
    alternative_product_id UNIQUEIDENTIFIER NOT NULL,
    reason NVARCHAR(255) NULL,
    similarity_score DECIMAL(5,2) NULL,
    created_at DATETIME DEFAULT GETDATE()
);

ALTER TABLE drug_alternatives
ADD CONSTRAINT FK_DrugAlternative_Product
FOREIGN KEY(product_id)
REFERENCES products(id);

ALTER TABLE drug_alternatives
ADD CONSTRAINT FK_DrugAlternative_Alternative
FOREIGN KEY(alternative_product_id)
REFERENCES products(id);

IF OBJECT_ID('prescription_dispense_items', 'U') IS NULL
BEGIN
    CREATE TABLE prescription_dispense_items
    (
        id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        prescription_item_id UNIQUEIDENTIFIER NOT NULL,
        product_id UNIQUEIDENTIFIER NOT NULL,
        quantity_dispensed INT NOT NULL,
        dispensed_by UNIQUEIDENTIFIER NULL,
        dispensed_at DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT PK_prescription_dispense_items PRIMARY KEY (id),

        CONSTRAINT FK_prescription_dispense_items_prescription_item
            FOREIGN KEY (prescription_item_id)
            REFERENCES prescription_items(id)
            ON DELETE CASCADE,

        CONSTRAINT FK_prescription_dispense_items_product
            FOREIGN KEY (product_id)
            REFERENCES products(id),

        CONSTRAINT FK_prescription_dispense_items_user
            FOREIGN KEY (dispensed_by)
            REFERENCES users(id)
            ON DELETE SET NULL,

        CONSTRAINT CK_prescription_dispense_items_quantity
            CHECK (quantity_dispensed > 0),

        CONSTRAINT UQ_prescription_dispense_items_prescription_item
            UNIQUE (prescription_item_id)
    );
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_prescription_dispense_items_product_id'
      AND object_id = OBJECT_ID('prescription_dispense_items')
)
BEGIN
    CREATE INDEX IX_prescription_dispense_items_product_id
    ON prescription_dispense_items(product_id);
END;

CREATE TABLE ecommerce_orders (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER REFERENCES users(id) ON DELETE SET NULL, 
    guest_phone NVARCHAR(20),
    shipping_address NVARCHAR(MAX) NOT NULL, 
    total_amount DECIMAL(12,2) NOT NULL,
    payment_method NVARCHAR(50) NOT NULL, 
    payment_status NVARCHAR(20) DEFAULT 'pending',
    order_status NVARCHAR(20) DEFAULT 'processing',
    prescription_id UNIQUEIDENTIFIER REFERENCES prescriptions(id) ON DELETE SET NULL,
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE order_items (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    order_id UNIQUEIDENTIFIER NOT NULL REFERENCES ecommerce_orders(id) ON DELETE CASCADE,
    product_id UNIQUEIDENTIFIER NOT NULL REFERENCES products(id),
    quantity INT NOT NULL CHECK (quantity > 0), 
    unit_price DECIMAL(12,2) NOT NULL
);

-- =========================================
-- NHÓM 12: QUẢN LÝ KHO (CHUẨN GPP/FEFO)
-- =========================================
CREATE TABLE suppliers (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    supplier_name NVARCHAR(255) NOT NULL, 
    contact_phone NVARCHAR(20),
    gdp_certified BIT DEFAULT 1, 
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE inventory_receipts (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    supplier_id UNIQUEIDENTIFIER REFERENCES suppliers(id),
    
    -- Phân quyền giám sát quy trình nhập thuốc
    created_by UNIQUEIDENTIFIER REFERENCES users(id),  -- Dược sĩ lập phiếu
    approved_by UNIQUEIDENTIFIER REFERENCES users(id), -- Admin duyệt phiếu
    
    total_cost DECIMAL(12,2) DEFAULT 0, 
    receipt_date DATETIME DEFAULT GETDATE(),
    
    -- Trạng thái duyệt phiếu
    status NVARCHAR(20) DEFAULT 'pending_approval' CHECK (status IN ('pending_approval', 'approved', 'rejected'))
);

CREATE TABLE receipt_details (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    receipt_id UNIQUEIDENTIFIER REFERENCES inventory_receipts(id) ON DELETE CASCADE,
    product_id UNIQUEIDENTIFIER REFERENCES products(id),
    batch_number NVARCHAR(100) NOT NULL, 
    expiration_date DATE NOT NULL, 
    quantity_imported INT NOT NULL CHECK (quantity_imported > 0),
    import_price DECIMAL(12,2) NOT NULL
);

-- =========================================
-- NHÓM 13: AI CHẨN ĐOÁN & KÊ TOA TỰ ĐỘNG
-- =========================================
CREATE TABLE symptom_logs (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    patient_id UNIQUEIDENTIFIER REFERENCES patient_profiles(id),
    symptoms_description NVARCHAR(MAX) NOT NULL, 
    predicted_disease NVARCHAR(255),
    severity_level NVARCHAR(50) CHECK (severity_level IN ('LOW', 'HIGH')),
    ai_advice NVARCHAR(MAX), 
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE auto_prescriptions (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    disease_name NVARCHAR(255) NOT NULL UNIQUE,
    recommended_product_id UNIQUEIDENTIFIER REFERENCES products(id), 
    dosage_instructions NVARCHAR(MAX)
);

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ecommerce_orders_status_created'
      AND object_id = OBJECT_ID('ecommerce_orders')
)
BEGIN
    CREATE INDEX IX_ecommerce_orders_status_created
    ON ecommerce_orders(order_status, created_at)
    INCLUDE(total_amount, user_id, guest_phone, payment_method, payment_status);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_inventory_receipts_status_date'
      AND object_id = OBJECT_ID('inventory_receipts')
)
BEGIN
    CREATE INDEX IX_inventory_receipts_status_date
    ON inventory_receipts(status, receipt_date)
    INCLUDE(total_cost, supplier_id);
END;