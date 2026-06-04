import { useState } from 'react';
import { AppShell, Burger, Group, Title, NavLink, ThemeIcon, Text } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconLayoutDashboard, IconBook, IconUsers, IconClipboardList } from '@tabler/icons-react';
import Dashboard from './pages/Dashboard';
import BookManagement from './pages/BookManagement';
import ReaderManagement from './pages/ReaderManagement';
import LoanManagement from './pages/LoanManagement';

export default function App() {
  const [opened, { toggle }] = useDisclosure();
  const [activeTab, setActiveTab] = useState<string>('dashboard');
  const [openModalOnPage, setOpenModalOnPage] = useState<boolean>(false);

  const handleNavigate = (tab: string, autoOpenModal: boolean = false) => {
    setActiveTab(tab);
    setOpenModalOnPage(autoOpenModal);
  };

  const renderActivePage = () => {
    switch (activeTab) {
      case 'dashboard':
        return <Dashboard onNavigate={handleNavigate} />;
      case 'books':
        return <BookManagement autoOpenAdd={openModalOnPage} onCloseAutoOpen={() => setOpenModalOnPage(false)} />;
      case 'readers':
        return <ReaderManagement autoOpenAdd={openModalOnPage} onCloseAutoOpen={() => setOpenModalOnPage(false)} />;
      case 'loans':
        return <LoanManagement autoOpenAdd={openModalOnPage} onCloseAutoOpen={() => setOpenModalOnPage(false)} />;
      default:
        return <Dashboard onNavigate={handleNavigate} />;
    }
  };

  const navItems = [
    { value: 'dashboard', label: 'Tổng quan', icon: IconLayoutDashboard, color: 'blue' },
    { value: 'books', label: 'Quản lý Sách', icon: IconBook, color: 'teal' },
    { value: 'readers', label: 'Quản lý Độc giả', icon: IconUsers, color: 'grape' },
    { value: 'loans', label: 'Quản lý Mượn trả', icon: IconClipboardList, color: 'indigo' },
  ];

  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 260,
        breakpoint: 'sm',
        collapsed: { mobile: !opened },
      }}
      padding="md"
      style={{
        backgroundColor: '#f8f9fa',
      }}
    >
      <AppShell.Header style={{ borderBottom: '1px solid #e9ecef', backgroundColor: '#ffffff' }}>
        <Group h="100%" px="md">
          <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
          <Group justify="space-between" style={{ flex: 1 }}>
            <Group gap="xs">
              <ThemeIcon size="lg" radius="md" variant="gradient" gradient={{ from: 'blue', to: 'cyan' }}>
                <IconBook size="1.2rem" />
              </ThemeIcon>
              <Title order={3} fw={700} style={{ letterSpacing: '-0.5px' }}>
                Thư viện Việt Đức
              </Title>
            </Group>
            <Text size="xs" c="dimmed" visibleFrom="xs">
              Phiên bản 1.0.0
            </Text>
          </Group>
        </Group>
      </AppShell.Header>

      <AppShell.Navbar p="md" style={{ borderRight: '1px solid #e9ecef', backgroundColor: '#ffffff' }}>
        {navItems.map((item) => {
          const Icon = item.icon;
          return (
            <NavLink
              key={item.value}
              active={activeTab === item.value}
              label={item.label}
              leftSection={
                <ThemeIcon size="sm" variant={activeTab === item.value ? 'filled' : 'light'} color={item.color}>
                  <Icon size="0.9rem" />
                </ThemeIcon>
              }
              onClick={() => {
                setActiveTab(item.value);
                setOpenModalOnPage(false);
                if (opened) toggle(); // Close mobile navbar when tab clicked
              }}
              style={{
                borderRadius: '8px',
                marginBottom: '4px',
                fontWeight: activeTab === item.value ? 600 : 500,
              }}
            />
          );
        })}
      </AppShell.Navbar>

      <AppShell.Main>
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '10px 0' }}>
          {renderActivePage()}
        </div>
      </AppShell.Main>
    </AppShell>
  );
}