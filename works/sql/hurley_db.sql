CREATE TABLE public.issues
(
    id          uuid NOT NULL DEFAULT gen_random_uuid(),
    title       text NOT NULL,
    description text,
    severity    text NOT NULL,
    status      text NOT NULL,
    created_at  timestamp without time zone NOT NULL,
    resolved_at timestamp without time zone,
    CONSTRAINT issues_pkey PRIMARY KEY (id)
);

INSERT INTO "public"."issues" ("id", "title", "description", "severity", "status", "created_at", "resolved_at")
VALUES (gen_random_uuid(), 'Cracked tile in lobby floor',
        'One ceramic tile in front entrance is cracked and uneven. Safety concern for foot traffic.', 'Low', 'Resolved',
        '2024-06-13 00:00:00', '2024-06-27 00:00:00'),
       (gen_random_uuid(), 'Leaking pipe in basement utility room',
        'Water leak detected during routine inspection. Needs immediate attention before drywall installation.', 'High',
        'Open', '2024-06-17 00:00:00', null),
       (gen_random_uuid(), 'Missing electrical outlet in office 203',
        'Plans show a power outlet on the north wall - not present during current walkthrough.', 'Medium', 'InProgress',
        '2024-06-19 00:00:00', null);