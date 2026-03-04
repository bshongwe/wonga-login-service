import type { Metadata } from 'next';
import './styling/globals.css';

export const metadata: Metadata = {
  title: 'Wonga Login Service',
  description: 'Login and registration platform',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
