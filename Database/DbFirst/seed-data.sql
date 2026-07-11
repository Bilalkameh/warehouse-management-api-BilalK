-- Suppliers

INSERT INTO "Suppliers"
(
    "Id",
    "Name",
    "Country",
    "ContactEmail",
    "PhoneNumber",
    "IsActive",
    "CreatedAt",
    "LastUpdatedAt"
)
VALUES
    (
        '11111111-1111-1111-1111-111111111111',
        'sup1',
        'lebanon',
        'sup1@mail.com',
        '+961-81-123-456',
        true,
        '2026-07-01 09:00:00+03',
        '2026-07-01 09:00:00+03'
    ),
    (
        '22222222-2222-2222-2222-222222222222',
        'sup2',
        'lebanon',
        'sup2@mail.com',
        '+961-71-654-123',
        true,
        '2026-07-01 09:05:00+03',
        '2026-07-01 09:05:00+03'
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        'sup3',
        'usa',
        'sup3@mail.com',
        '+1-123-456',
        true,
        '2026-07-01 09:10:00+03',
        '2026-07-01 09:10:00+03'
    ),
    (
        '44444444-4444-4444-4444-444444444444',
        'sup4',
        'germany',
        'sup4@mail.com',
        '+49-123-333',
        true,
        '2026-07-01 09:15:00+03',
        '2026-07-01 09:15:00+03'
    );


-- Products

INSERT INTO "Products"
(
    "Id",
    "Name",
    "SKU",
    "Description",
    "Price",
    "QuantityInStock",
    "ExpiryDate",
    "IsArchived",
    "CreatedAt",
    "LastUpdatedAt",
    "SupplierId"
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1',
        'laptop1',
        'LAP-001',
        'powerful gaming laptop',
        1299.9,
        10,
        '2029-02-07 00:00:00+03',
        false,
        '2026-07-02 10:00:00+03',
        '2026-07-02 10:00:00+03',
        '11111111-1111-1111-1111-111111111111'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2',
        'laptop2',
        'LAP-002',
        'budget laptop',
        500,
        12,
        '2028-05-30 00:00:00+03',
        false,
        '2026-07-02 10:05:00+03',
        '2026-07-02 10:05:00+03',
        '22222222-2222-2222-2222-222222222222'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3',
        'mouse1',
        'MOU-001',
        'budget mouse',
        10,
        30,
        '2028-01-01 00:00:00+03',
        false,
        '2026-07-02 10:10:00+03',
        '2026-07-02 10:10:00+03',
        '11111111-1111-1111-1111-111111111111'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4',
        'mouse2',
        'MOU-002',
        'gaming mouse',
        40,
        15,
        '2028-01-01 00:00:00+03',
        false,
        '2026-07-02 10:15:00+03',
        '2026-07-02 10:15:00+03',
        '33333333-3333-3333-3333-333333333333'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5',
        'keyboard1',
        'KEY-001',
        'gaming keyboard',
        60.99,
        20,
        '2029-01-01 00:00:00+03',
        false,
        '2026-07-02 10:20:00+03',
        '2026-07-02 10:20:00+03',
        '22222222-2222-2222-2222-222222222222'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6',
        'keyboard2',
        'KEY-002',
        'budget keyboard',
        20,
        25,
        '2029-06-30 00:00:00+03',
        false,
        '2026-07-02 10:25:00+03',
        '2026-07-02 10:25:00+03',
        '44444444-4444-4444-4444-444444444444'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7',
        'scanner1',
        'SCN-001',
        'scanner 1',
        100,
        8,
        '2029-04-01 00:00:00+03',
        false,
        '2026-07-02 10:30:00+03',
        '2026-07-02 10:30:00+03',
        '33333333-3333-3333-3333-333333333333'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8',
        'printer1',
        'PRI-001',
        'printer 1',
        120.99,
        7,
        '2028-05-01 00:00:00+03',
        false,
        '2026-07-02 10:35:00+03',
        '2026-07-02 10:35:00+03',
        '11111111-1111-1111-1111-111111111111'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9',
        'monitor1',
        'MON-001',
        'OLED monitor',
        450.99,
        5,
        '2029-01-01 00:00:00+03',
        false,
        '2026-07-02 10:40:00+03',
        '2026-07-02 10:40:00+03',
        '22222222-2222-2222-2222-222222222222'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10',
        'monitor2',
        'MON-002',
        'budget monitor',
        100.99,
        17,
        '2029-01-01 00:00:00+03',
        false,
        '2026-07-02 10:45:00+03',
        '2026-07-02 10:45:00+03',
        '44444444-4444-4444-4444-444444444444'
    );


-- Product Images

INSERT INTO "ProductImages"
(
    "Id",
    "ProductId",
    "FileName",
    "FilePath"
)
VALUES
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1',
        'laptop1.jpg',
        'uploads/laptop1.jpg'
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3',
        'mouse1.jpg',
        'uploads/mouse1.jpg'
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9',
        'monitor1.jpg',
        'uploads/monitor1.jpg'
    );