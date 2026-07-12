import { Header } from "@/components/storefront/Header";
import { Footer } from "@/components/storefront/Footer";
import { WhatsAppButton } from "@/components/storefront/WhatsAppButton";

export default function StorefrontLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex min-h-full flex-col">
      <Header />
      <main className="flex-1">{children}</main>
      <Footer />
      <WhatsAppButton />
    </div>
  );
}
