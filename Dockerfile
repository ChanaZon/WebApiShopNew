# Use official Redis image
FROM redis:7.2-alpine

# הגדר סיסמה לרדיס ישירות בדוקר פייל
CMD ["redis-server", "--requirepass", "MyRedisPassword123!"]

