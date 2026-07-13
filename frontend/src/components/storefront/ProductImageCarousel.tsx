"use client";

import { useState } from "react";
import { resolveMediaUrl } from "@/lib/media";
import type { ProductImageDto } from "@/lib/types";

interface ProductImageCarouselProps {
  images: ProductImageDto[];
  fallbackLabel: string;
  alt: string;
}

export function ProductImageCarousel({ images, fallbackLabel, alt }: ProductImageCarouselProps) {
  const [index, setIndex] = useState(0);

  if (images.length === 0) {
    return (
      <div className="flex aspect-square items-center justify-center rounded-lg bg-zinc-100 dark:bg-zinc-800">
        <span className="text-lg text-zinc-400">{fallbackLabel}</span>
      </div>
    );
  }

  function goTo(next: number) {
    setIndex((next + images.length) % images.length);
  }

  const currentUrl = resolveMediaUrl(images[index].url) ?? undefined;

  return (
    <div>
      <div className="relative flex aspect-square items-center justify-center overflow-hidden rounded-lg bg-zinc-100 dark:bg-zinc-800">
        {/* eslint-disable-next-line @next/next/no-img-element */}
        <img src={currentUrl} alt={alt} className="h-full w-full object-cover" />

        {images.length > 1 && (
          <>
            <button
              type="button"
              onClick={() => goTo(index - 1)}
              aria-label="Imagem anterior"
              className="absolute left-3 top-1/2 flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-full bg-white/70 text-zinc-900 backdrop-blur hover:bg-white dark:bg-zinc-900/70 dark:text-white dark:hover:bg-zinc-900"
            >
              ‹
            </button>
            <button
              type="button"
              onClick={() => goTo(index + 1)}
              aria-label="Próxima imagem"
              className="absolute right-3 top-1/2 flex h-9 w-9 -translate-y-1/2 items-center justify-center rounded-full bg-white/70 text-zinc-900 backdrop-blur hover:bg-white dark:bg-zinc-900/70 dark:text-white dark:hover:bg-zinc-900"
            >
              ›
            </button>

            <div className="absolute bottom-3 left-1/2 flex -translate-x-1/2 gap-2">
              {images.map((image, i) => (
                <button
                  key={image.id}
                  type="button"
                  onClick={() => goTo(i)}
                  aria-label={`Imagem ${i + 1} de ${images.length}`}
                  className={`h-1.5 rounded-full transition-all ${
                    i === index ? "w-6 bg-blue-600" : "w-1.5 bg-zinc-400/70"
                  }`}
                />
              ))}
            </div>
          </>
        )}
      </div>

      {images.length > 1 && (
        <div className="mt-3 flex gap-2 overflow-x-auto">
          {images.map((image, i) => (
            <button
              key={image.id}
              type="button"
              onClick={() => goTo(i)}
              aria-label={`Ver imagem ${i + 1}`}
              className={`h-16 w-16 shrink-0 overflow-hidden rounded-md border-2 ${
                i === index ? "border-blue-600" : "border-transparent"
              }`}
            >
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={resolveMediaUrl(image.url) ?? undefined} alt="" className="h-full w-full object-cover" />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
