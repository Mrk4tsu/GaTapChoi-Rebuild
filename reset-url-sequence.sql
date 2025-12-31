-- Ch?y script này trong PostgreSQL ?? reset sequence c?a b?ng urls
-- Cách 1: Reset sequence v? giá tr? max hi?n t?i + 1
SELECT setval(pg_get_serial_sequence('urls', 'id'), COALESCE(MAX(id), 1), true) FROM urls;

-- Ho?c Cách 2: N?u b?ng r?ng, reset v? 1
-- SELECT setval(pg_get_serial_sequence('urls', 'id'), 1, false);

-- Ki?m tra sequence hi?n t?i
SELECT currval(pg_get_serial_sequence('urls', 'id'));
