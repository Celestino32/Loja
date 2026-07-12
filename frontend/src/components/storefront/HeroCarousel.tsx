"use client";

import Link from "next/link";
import { useEffect, useState } from "react";

interface Slide {
  title: string;
  description: string;
  href: string;
  cta: string;
  gradient: string;
}

const slides: Slide[] = [
  {
    title: "Eletrónica de confiança, entregue em toda Angola.",
    description: "Smartphones, computadores e eletrodomésticos com garantia oficial.",
    href: "/produtos",
    cta: "Ver todos os produtos",
    gradient: "from-blue-700 via-blue-600 to-zinc-900",
  },
  {
    title: "Pague do seu jeito.",
    description: "Multicaixa Express, referência de pagamento ou transferência bancária.",
    href: "/produtos",
    cta: "Explorar loja",
    gradient: "from-zinc-900 via-zinc-800 to-blue-900",
  },
  {
    title: "Entrega em todas as províncias.",
    description: "Ou retire gratuitamente na loja — você escolhe no checkout.",
    href: "/produtos",
    cta: "Começar a comprar",
    gradient: "from-blue-900 via-zinc-900 to-zinc-950",
  },
];

export function HeroCarousel() {
  const [index, setIndex] = useState(0);

  useEffect(() => {
    const timer = setInterval(() => setIndex((i) => (i + 1) % slides.length), 6000);
    return () => clearInterval(timer);
  }, []);

  function goTo(next: number) {
    setIndex((next + slides.length) % slides.length);
  }

  return (
    <section className="relative overflow-hidden">
      {slides.map((slide, i) => (
        <div
          key={slide.title}
          aria-hidden={i !== index}
          className={`bg-gradient-to-br ${slide.gradient} transition-opacity duration-500 ${
            i === index ? "relative opacity-100" : "absolute inset-0 opacity-0"
          }`}
        >
          <div className="mx-auto max-w-6xl px-4 py-16 sm:px-6 sm:py-20">
            <h1 className="max-w-xl text-3xl font-bold text-white sm:text-4xl">{slide.title}</h1>
            <p className="mt-4 max-w-lg text-zinc-200">{slide.description}</p>
            <Link
              href={slide.href}
              className="mt-6 inline-block rounded-md bg-white px-6 py-3 text-sm font-semibold text-zinc-900 hover:bg-zinc-100"
            >
              {slide.cta}
            </Link>
          </div>
        </div>
      ))}

      <button
        type="button"
        onClick={() => goTo(index - 1)}
        aria-label="Anterior"
        className="absolute left-3 top-1/2 hidden h-9 w-9 -translate-y-1/2 items-center justify-center rounded-full bg-white/20 text-white backdrop-blur hover:bg-white/30 sm:flex"
      >
        ‹
      </button>
      <button
        type="button"
        onClick={() => goTo(index + 1)}
        aria-label="Próximo"
        className="absolute right-3 top-1/2 hidden h-9 w-9 -translate-y-1/2 items-center justify-center rounded-full bg-white/20 text-white backdrop-blur hover:bg-white/30 sm:flex"
      >
        ›
      </button>

      <div className="absolute bottom-4 left-1/2 flex -translate-x-1/2 gap-2">
        {slides.map((slide, i) => (
          <button
            key={slide.title}
            type="button"
            onClick={() => goTo(i)}
            aria-label={`Slide ${i + 1} de ${slides.length}`}
            className={`h-1.5 rounded-full transition-all ${i === index ? "w-6 bg-white" : "w-1.5 bg-white/50"}`}
          />
        ))}
      </div>
    </section>
  );
}
