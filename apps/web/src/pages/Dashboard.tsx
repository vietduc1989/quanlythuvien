import { useEffect, useState } from 'react';
import { Card, Text, SimpleGrid, Group, Paper, Button, Title, ThemeIcon, LoadingOverlay, Box } from '@mantine/core';
import { IconBook, IconUsers, IconClipboardList, IconAlertTriangle, IconPlus, IconAddressBook, IconArrowsRightLeft } from '@tabler/icons-react';
import axiosClient from '../api/apiClient';

interface DashboardStats {
  totalBooks: number;
  totalReaders: number;
  totalLoans: number;
  overdueLoans: number;
}

interface DashboardProps {
  onNavigate: (tab: string, autoOpenModal?: boolean) => void;
}

export default function Dashboard({ onNavigate }: DashboardProps) {
  const [stats, setStats] = useState<DashboardStats>({
    totalBooks: 0,
    totalReaders: 0,
    totalLoans: 0,
    overdueLoans: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchStats = async () => {
      setLoading(true);
      setError(null);
      try {
        // Fetch books count
        const booksRes = await axiosClient.get('/Books', { params: { pageSize: 1 } });
        const booksCount = booksRes.data.data?.totalCount ?? 0;

        // Fetch readers count
        const readersRes = await axiosClient.get('/Readers', { params: { pageSize: 1 } });
        const readersCount = readersRes.data.data?.totalCount ?? 0;

        // Fetch loans count
        const loansRes = await axiosClient.get('/Loans', { params: { pageSize: 1 } });
        const loansCount = loansRes.data.data?.totalCount ?? 0;

        // Fetch overdue loans count
        const overdueRes = await axiosClient.get('/Loans', { params: { isOverdue: true, pageSize: 1 } });
        const overdueCount = overdueRes.data.data?.totalCount ?? 0;

        setStats({
          totalBooks: booksCount,
          totalReaders: readersCount,
          totalLoans: loansCount,
          overdueLoans: overdueCount,
        });
      } catch (err: any) {
        console.error('Failed to load dashboard stats:', err);
        setError('Không thể tải dữ liệu thống kê từ hệ thống.');
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  const statCards = [
    {
      title: 'Tổng số sách',
      value: stats.totalBooks,
      icon: IconBook,
      color: 'blue',
      description: 'Đầu sách hiện có trong thư viện',
    },
    {
      title: 'Độc giả',
      value: stats.totalReaders,
      icon: IconUsers,
      color: 'teal',
      description: 'Độc giả đã đăng ký tài khoản',
    },
    {
      title: 'Lượt mượn sách',
      value: stats.totalLoans,
      icon: IconClipboardList,
      color: 'indigo',
      description: 'Tổng số phiếu mượn đã lập',
    },
    {
      title: 'Phiếu quá hạn',
      value: stats.overdueLoans,
      icon: IconAlertTriangle,
      color: stats.overdueLoans > 0 ? 'red' : 'gray',
      description: 'Phiếu mượn chưa trả quá hạn',
    },
  ];

  return (
    <Box style={{ position: 'relative', minHeight: '400px' }}>
      <LoadingOverlay visible={loading} overlayProps={{ blur: 2 }} />

      <Group justify="space-between" mb="xl">
        <div>
          <Title order={2} fw={700} style={{ fontFamily: 'Outfit, sans-serif' }}>
            Tổng quan hệ thống
          </Title>
          <Text size="sm" c="dimmed">
            Theo dõi hoạt động và các chỉ số thống kê của thư viện
          </Text>
        </div>
      </Group>

      {error && (
        <Paper p="md" mb="xl" withBorder style={{ borderColor: 'red', backgroundColor: '#fff5f5' }}>
          <Text c="red" size="sm" fw={500}>
            {error}
          </Text>
        </Paper>
      )}

      <SimpleGrid cols={{ base: 1, sm: 2, lg: 4 }} spacing="md" mb="xl">
        {statCards.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Paper key={index} p="md" withBorder radius="md" shadow="xs" style={{ overflow: 'hidden' }}>
              <Group justify="space-between" mb="xs">
                <Text size="xs" c="dimmed" fw={700} style={{ textTransform: 'uppercase' }}>
                  {stat.title}
                </Text>
                <ThemeIcon color={stat.color} variant="light" size="lg" radius="md">
                  <Icon size="1.2rem" />
                </ThemeIcon>
              </Group>

              <Group align="flex-end" gap="xs" mt="xs">
                <Text size="2rem" fw={700} style={{ lineHeight: 1 }}>
                  {stat.value}
                </Text>
              </Group>

              <Text size="xs" c="dimmed" mt="sm">
                {stat.description}
              </Text>
            </Paper>
          );
        })}
      </SimpleGrid>

      <Title order={3} mb="md" mt="xl" fw={600}>
        Thao tác nhanh
      </Title>
      <SimpleGrid cols={{ base: 1, sm: 3 }} spacing="md">
        <Card withBorder radius="md" shadow="sm" padding="lg">
          <ThemeIcon size="xl" radius="md" color="blue" variant="light" mb="md">
            <IconPlus size="1.5rem" />
          </ThemeIcon>
          <Text fw={600} mb="xs">Thêm sách mới</Text>
          <Text size="sm" c="dimmed" mb="md" style={{ flexGrow: 1 }}>
            Đăng ký thêm đầu sách mới vào cơ sở dữ liệu thư viện để phục vụ bạn đọc.
          </Text>
          <Button fullWidth onClick={() => onNavigate('books', true)} variant="light" color="blue">
            Quản lý sách
          </Button>
        </Card>

        <Card withBorder radius="md" shadow="sm" padding="lg">
          <ThemeIcon size="xl" radius="md" color="teal" variant="light" mb="md">
            <IconAddressBook size="1.5rem" />
          </ThemeIcon>
          <Text fw={600} mb="xs">Đăng ký độc giả</Text>
          <Text size="sm" c="dimmed" mb="md" style={{ flexGrow: 1 }}>
            Lập thẻ độc giả mới, cập nhật hồ sơ thông tin liên lạc và gia hạn thẻ.
          </Text>
          <Button fullWidth onClick={() => onNavigate('readers', true)} variant="light" color="teal">
            Quản lý độc giả
          </Button>
        </Card>

        <Card withBorder radius="md" shadow="sm" padding="lg">
          <ThemeIcon size="xl" radius="md" color="indigo" variant="light" mb="md">
            <IconArrowsRightLeft size="1.5rem" />
          </ThemeIcon>
          <Text fw={600} mb="xs">Lập phiếu mượn trả</Text>
          <Text size="sm" c="dimmed" mb="md" style={{ flexGrow: 1 }}>
            Thực hiện cho bạn đọc mượn sách, ghi nhận sách trả và kiểm tra tình trạng quá hạn.
          </Text>
          <Button fullWidth onClick={() => onNavigate('loans', true)} variant="light" color="indigo">
            Quản lý mượn trả
          </Button>
        </Card>
      </SimpleGrid>
    </Box>
  );
}
