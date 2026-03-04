import type { Metadata } from "next";
import React, { type ReactNode } from "react";
import { Inter } from "next/font/google";
import "./styling/globals.css";
import AppLayout from "@/components/AppLayout";
import { AuthProvider } from "@/contexts/AuthContext";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Wonga Login Service",
  description: "Secure authentication platform",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <AuthProvider>
          <AppLayout>{children}</AppLayout>
        </AuthProvider>
      </body>
    </html>
  );
}
