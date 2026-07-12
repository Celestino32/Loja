export function StarRating({ value, size = 16 }: { value: number; size?: number }) {
  return (
    <div className="flex items-center gap-0.5" aria-label={`${value} de 5 estrelas`}>
      {[1, 2, 3, 4, 5].map((star) => (
        <svg
          key={star}
          viewBox="0 0 20 20"
          width={size}
          height={size}
          fill={star <= Math.round(value) ? "#f59e0b" : "none"}
          stroke="#f59e0b"
          strokeWidth="1.2"
          aria-hidden="true"
        >
          <path d="M10 1.5l2.6 5.3 5.8.8-4.2 4.1 1 5.8-5.2-2.7-5.2 2.7 1-5.8L1.6 7.6l5.8-.8L10 1.5Z" />
        </svg>
      ))}
    </div>
  );
}
